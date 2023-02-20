module HelloPage

open Elmish
open Elmish.Navigation
open Elmish.React
open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.UseElmish
open Browser.Types
open Types

[<ReactComponent>]
let HelloPage (props: HelloPage.Props) =
    let name, setName = React.useState ""
    let page, setPage = React.useState Hello

    match page with
    | Hello ->
        class' "helloPage" Html.div [
            class' "header" Html.span [
                classTxt' "greeting" Html.div $"Hello! What's your name?"
                classP' "settings" Html.button [prop.onClick (thunk1 setPage Settings); prop.type'.button; prop.text $"Settings"]
                ]

            classP' "inputArea" Html.form [
                prop.children [
                    Html.input [prop.type'.text; prop.valueOrDefault name; prop.onChange setName; prop.autoFocus true]
                    Html.button [prop.className "startButton"; prop.type'.submit; prop.text "Start"]
                    ]
                prop.onSubmit (fun e ->
                    e.preventDefault()
                    setPage Main
                    )
                ]
            HighScore.Component (props.scores, None) ignore
            ]
    | Main ->
        Main.Export.Component name (thunk1 setPage Hello)
    | Settings ->
        Settings.Component () (thunk1 setPage Hello)
    | HighScore ->
        HighScore.Component (props.scores, thunk1 setPage Hello |> Some) ignore
