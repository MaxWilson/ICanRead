module Scoreboard

open DataContracts
open Microsoft.Azure.Cosmos
open System.Linq
open System.Threading.Tasks
open System.Net
open Microsoft.Extensions.Logging
open System.Runtime.Caching
open System

let queryAsyncBase<'t> (top: int option) (sqlTxt: string) (container: Container) =
    let rec readResults countSoFar (iter: _ FeedIterator) = task {
        if iter.HasMoreResults then
            let! r = iter.ReadNextAsync()
            let results = (r |> Array.ofSeq)
            match top with
            | Some top when countSoFar + results.Length > top ->
                return results |> Seq.take (top - countSoFar)
            | _ ->
                let! more = readResults results.Length iter
                return Seq.append results more
        else
            return []
        }

    task {
        let iter = container.GetItemQueryIterator<'t>(sqlTxt)
        let! results = readResults 0 iter
        return results |> Array.ofSeq
    }

let queryAsync<'t> sqlTxt container = queryAsyncBase<'t> None sqlTxt container
let queryAsyncN<'t> top sqlTxt container = queryAsyncBase<'t> (Some top) sqlTxt container

let normalize (dt: DateTimeOffset) = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dt, "PST").ToString("yyyy-MM-dd")

module Caching =
    // We embrace eventual consistency for score lists in the name of decent perf:
    // we will load info for the top 5 players ever and the top 5 players over the last 7 days,
    // but only when the cached data is more than 60 minutes old.
    // Implication: sometimes the scores you see in the UI could be slightly out of date,
    // and the UI should be careful with its own caching to make sure player's own scores
    // at least are up to date.
    // We could have used SQL Azure serverless compute instead and queried every time,
    // but I anticipate that a high-scores list is kind of an edge case and I don't want
    // to pay for SQL Azure every month just for this edge case.

    // eventual consistency! Will not pick up score writes immediately unless you force refresh.
    let topPlayers(scores:Container, forceRefresh) = task {
        match MemoryCache.Default.Get("topPlayers") with
        | :? HighScores as cached when not forceRefresh ->
            return cached
        | _ ->
            let! bestEverPlayers =
                scores
                |> queryAsyncN<{| name: string |}> 5 "select distinct s.name from s order by s.score desc"
                |> Task.map (Array.map (fun r -> r.name))
            let dt = System.DateTimeOffset.UtcNow |> normalize
            let nameList names =
                let escape (s:string) = s.Replace("'", "\'")
                String.join "," (names |> Seq.map (fun n -> $"'{escape n}'"))
            let! bestRecentPlayers =
                scores
                |> queryAsyncN<{| name: string |}> 5 $"select distinct s.name from s where s.normalizedDate >= '{dt}' order by s.score desc "
                |> Task.map (Array.map (fun r -> r.name))

            let! bestEver = scores |> queryAsyncN<Row> 5 $"select s.name, max(s.score) as score from s where s.name in ({nameList bestEverPlayers}) group by s.name"
            let! bestRecent = scores |> queryAsyncN<Row> 5 $"select s.name, max(s.score) as score from s where s.normalizedDate >= '{System.DateTimeOffset.UtcNow.AddDays(-8) |> normalize}' and s.name in ({nameList bestEverPlayers}) group by s.name"
            let best = { allTime = bestEver; recent = bestRecent }

            // make sure to update cache
            MemoryCache.Default.Add("topPlayers", best, DateTimeOffset.Now.AddMinutes(60.)) |> ignore
            return best
        }

    let bestToday key fallback =
        task {
            match MemoryCache.Default.Get(key) with
            | :? Row as cached -> return cached.score
            | _ ->
                match! fallback() with
                | Some r -> return r.score
                | None -> return 0
            }

    let update key row' =
        MemoryCache.Default.Add(key, row', DateTimeOffset.Now.AddDays(1.)) |> ignore


// will read from CosmosDB if the item exists, or return None if it doesn't. Does not handle other errors like being over quota or network errors.
let tryGet (t: Task<ItemResponse<'t>>) = task {
    try
        let! t' = t
        return
            if t'.StatusCode = HttpStatusCode.OK then
                Some t'.Resource
            elif t'.StatusCode = HttpStatusCode.NotFound then
                None
            else
                failwithf "Unexpected status code %A" t'.StatusCode // todo: error handling, retries, etc.
    with :? CosmosException as err ->
        return
            if err.StatusCode = HttpStatusCode.NotFound then
                None
            else
                raise err
    }

let WriteScore(log:ILogger, row' :Row, cosmos: CosmosClient) = task {
    let userName = row'.name
    let score = row'.score
    let! db = cosmos.CreateDatabaseIfNotExistsAsync "ICanRead"
    let! scores = db.Database.CreateContainerIfNotExistsAsync("ScoresByDay", "/name")
    let date = DateTimeOffset.UtcNow
    let key = $"""{normalize date}-{match row'.name with s when String.IsNullOrWhiteSpace s -> "Anonymous" | name -> name}"""
    let! bestToday = Caching.bestToday key (fun _ -> scores.Container.ReadItemAsync<Row>(key, new PartitionKey(userName)) |> tryGet)

    if score > bestToday then
        do! scores.Container.UpsertItemAsync<_>({| id = key; name = row'.name; score = row'.score; normalizedDate = date |> normalize |}, new PartitionKey(userName)) |> Task.ignore
        Caching.update key row'
    }

let ReadScores(cosmos: CosmosClient, forceRefresh: bool) = task {
    let! db = cosmos.CreateDatabaseIfNotExistsAsync "ICanRead"
    let! scores = db.Database.CreateContainerIfNotExistsAsync("ScoresByDay", "/name")
    return! Caching.topPlayers(scores.Container, forceRefresh)
    }