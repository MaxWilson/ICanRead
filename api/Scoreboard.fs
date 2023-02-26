module Scoreboard

open DataContracts

open Thoth.Json.Net
open Dapper
open Dapper.FSharp
open Dapper.FSharp.MSSQL
open System.Data.SqlClient

open System.Net.Http
open System.Net

let highScores = table'<Row> "HighScore" |> inSchema "ICanRead"

let read(conn: SqlConnection) =
    task {
        do! conn.OpenAsync()
        let! bestEver =
            conn.QueryAsync<Row>(
                """
                select top 5 name, max(score) as score from ICanRead.HighScore h
                group by name
                order by max(score)
                """
                )
        let! bestRecent =
            conn.QueryAsync<Row>(
                """
                select top 5 name, max(score) as score from ICanRead.HighScore h
                where date >= dateadd(wk, -1, SYSDATETIMEOFFSET())
                group by name
                order by max(score)
                """
                )
        return { allTime = bestEver |> Array.ofSeq; recent = bestRecent |> Array.ofSeq }
    }

let insert(conn:SqlConnection, input: Row) =
    task {
        do! conn.OpenAsync()

        let! rowsAffected =
            insert {
                into highScores
                value input
            } |> conn.InsertAsync

        if rowsAffected = 0 then failwith "Unable to write for some reason"
        return ()
    }