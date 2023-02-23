module HighScores

open DataContracts

// we are NOT using the Elmish pattern for high scores, for perf reasons. We are using a global variable which is lazily updated with every write.
let mutable highScores = LocalStorage.HighScores.read()

// If we cannot read due to some bug or network outage,
// we try to fail in a way that won't persist and won't break the UI
let errorFallback() = { allTime = Array.empty; recent = Array.empty }
let mutable initializationPromise = None

open Fetch
open Fable.Core
open System

type GlobalFetch =
        [<Fable.Core.Global>]
        static member fetch (req: RequestInfo, ?init: RequestInit): JS.Promise<Response> = jsNative

let updateLocalStorageCache() =
    let p = promise {
                try
                    let! (v: HighScores) = Thoth.Fetch.Fetch.get "/api/ReadHighScores"
                    highScores <- Some v
                    LocalStorage.HighScores.write v
                    return v
                with err ->
                    log err
                    return errorFallback()
            }
    initializationPromise <- Some p

let readScores () =
    match highScores, initializationPromise with
    | None, None ->
        shouldntHappen() // App.fs should have called updateLocalStorageCache() before this
    | None, Some p ->
        Error {| awaiting = p |}
    | Some highScores, _ ->
        Ok highScores

let mutable listeners = []
let registerForUpdates =
    fun (listener: (HighScores -> unit)) ->
        listeners <- listener :: listeners

let mutable lastWrite = DateTimeOffset.Now

let writeScore (userName: string, score: int) =
    let userName = if System.String.IsNullOrWhiteSpace userName then "Anonymous" else userName
    // We don't really care if we lose 30 seconds' worth of progress. It will get saved if they either
    // quit or answer another question. Note: this isn't debounced, not guaranteed to save every 30
    // seconds, just guaranteed not to save more than once every 30 seconds.
    if (DateTimeOffset.Now - lastWrite).TotalSeconds > 30 then
        lastWrite <- DateTimeOffset.Now
        Thoth.Fetch.Fetch.post("api/WriteScore", { name = userName; score = score }) |> ignore
        // attempt to write but don't wait to see results

    let updateTables(highScores': HighScores) =
        if highScores'.allTime.Length < 5 || highScores'.recent |> Array.exists (fun r -> score >= r.score) then
            let update (rows: Row array) =
                if rows |> Array.exists (fun r -> r.name = userName) then
                    rows |> Array.map (fun r -> if r.name = userName then { r with score = score } else r)
                else
                    rows |> Array.append [| { name = userName; score = score } |] |> Array.take (min 5 (rows.Length + 1))
            let highScores' = { highScores' with recent = highScores'.recent |> update; allTime = highScores'.allTime |> update }
            highScores <- highScores' |> Some
            for notify in listeners do
                notify highScores'
    match readScores() with
    | Ok highScores ->
        updateTables highScores
    | Error e ->
        promise {
            let! highScores = e.awaiting
            updateTables highScores
            }
        |> ignore
