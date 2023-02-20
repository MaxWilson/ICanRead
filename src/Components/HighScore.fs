module HighScore
open Feliz
open Elmish
open Feliz.UseElmish
open Browser.Types
open System
open Fable.DateFunctions
open Types.HighScore

[<AutoOpen>]
module private Impl =
    open Types.HighScore

    type Model = {
        allTime: Row array
        recent: Row array
        }

    let init (allTime, recent) =
        {   allTime = allTime
            recent = recent }

    type Msg = Msg

    let update msg model =
        match msg with
        | Msg -> model

[<ReactComponent>]
let Component (args, (onQuit: (unit -> unit) option)) dispatch =
    let showRecent, setRecent = React.useState false
    let model, dispatch = React.useElmish(thunk3 Program.mkSimple init update ignore2, args)
    class' "highScores" Html.div [
        classTxt' "title" Html.div "High Scores"
        let scoresOf className title (rows: Row array) =
            class' className Html.div [
                classTxt' "header" Html.div title
                classTxt' "bolded" Html.div "Place"
                classTxt' "bolded" Html.div "Name"
                classTxt' "bolded" Html.div "Score"
                classTxt' "bolded" Html.div "Date"
                for ix, row in rows |> Array.mapi (fun i row -> i+1, row) do
                    Html.div (match ix with 1 -> "1st" | 2 -> "2nd" | 3 -> "3rd" | n -> $"{n}th")
                    Html.div row.name
                    Html.div row.score
                    Html.div (row.date.Format "MM/dd/yyyy")
                ]
        scoresOf "allTime" "All time" model.allTime
        scoresOf "recent" "This week" model.recent
        ]
