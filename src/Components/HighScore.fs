module HighScore
open Feliz
open Elmish
open Feliz.UseElmish
open Browser.Types
open System

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
let HighScores args onQuit =
    let showRecent, setRecent = React.useState false
    let model, dispatch = React.useElmish(thunk3 Program.mkSimple init update ignore2, args)
    class' "highScores" Html.div [
        class' "recent" Html.div [
            for row in model.recent do
                Html.div row.name
                Html.div row.score
                Html.div (row.date.Date.ToString())
            ]
        class' "allTime" Html.div [
            for row in model.allTime do
                Html.div row.name
                Html.div row.score
                Html.div (row.date.Date.ToString())
            ]
        ]
