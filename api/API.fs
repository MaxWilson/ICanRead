namespace Company.Function

open System
open System.IO
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Http
open Thoth.Json.Net
open Microsoft.Extensions.Logging
open System.Net.Http
open System.Net
open DataContracts
open System.Data.SqlClient

type API(conn:SqlConnection) =
    let toJsonResponse data =
        new HttpResponseMessage(
            HttpStatusCode.OK,
            Content = new StringContent(Encode.Auto.toString data))

    let fromJsonRequest (req: HttpRequest) = task {
        let! requestBody = ((new StreamReader(req.Body)).ReadToEndAsync())
        match Decode.Auto.fromString<'t>(requestBody) with
        | Ok data -> return data
        | Error err ->
            return $"Could not deserialize JSON because '{err}'" |> InvalidOperationException |> raise
        }

    [<FunctionName("ReadHighScores")>]
    member this.ReadHighScores(log: ILogger, [<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req: HttpRequest) = task {
        try
            // forceRefresh is optional, defaults to true
            return! Scoreboard.read(conn) |> Task.map toJsonResponse
        with err ->
            log.LogError(err.ToString());
            return raise err
        }

    [<FunctionName("WriteScore")>]
    member this.WriteScore(log:ILogger, [<HttpTrigger(AuthorizationLevel.Anonymous, "post")>] req: HttpRequest) = task {
        try
            let! (row: Row) = fromJsonRequest req
            do! Scoreboard.insert(conn, row)
        with err ->
            log.LogError(err.ToString());
            return raise err
        }
