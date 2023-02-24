module HighScore
open Feliz
open Elmish
open Feliz.UseElmish
open Browser.Types
open System
open Fable.DateFunctions
open Types.HighScore
open DataContracts

[<ReactComponent>]
let Component (props: Props) =
    let scores, setScores = React.useState None
    React.useEffectOnce(fun _ ->
        if props.registerForUpdates then
            HighScores.registerForUpdates (Some >> setScores)
        match HighScores.readScores() with
        | Ok scores -> setScores (Some scores)
        | Error err ->
            promise {
                let! highScores = err.awaiting
                setScores (Some highScores)
                } |> ignore
        )
    match scores with
    | None ->
        classTxt' "highScores" Html.div "Loading high scores..."
    | Some scores ->
        class' "highScores" Html.div [
            classTxt' "title" Html.div "High Scores"
            let scoresOf className title (rows: Row array) =
                class' className Html.div [
                    classTxt' "header" Html.div title
                    classTxt' "bolded" Html.div "Place"
                    classTxt' "bolded" Html.div "Name"
                    classTxt' "bolded" Html.div "Score"
                    for ix, row in rows |> Array.sortByDescending (fun r -> r.score) |> Array.mapi (fun i row -> i+1, row) do
                        if ix <= 5 then
                            Html.div (match ix with 1 -> "1st" | 2 -> "2nd" | 3 -> "3rd" | n -> $"{n}th")
                            Html.div (if String.isNullOrWhitespace row.name then "Anonymous" else row.name)
                            Html.div row.score
                    ]
            scoresOf "allTime" "All time" scores.allTime
            scoresOf "recent" "This week" scores.recent
            match props.onQuit with
            | None -> ()
            | Some quit ->
                class' "quit" Html.div [
                    Html.button [prop.text "OK"; prop.onClick (thunk1 quit ())]
                    ]
            ]
