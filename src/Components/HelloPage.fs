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
let HelloPage highScores =
    let name, setName = React.useState ""
    let page, setPage = React.useState Hello
#if DEBUG
    // workaround for strange issue in dev where state updates don't get flushed/UI doesn't update after setState. E.g. clicking the Start button does nothing until you open the Chrome dev console.
    let setPage x =
        setPage x
        Fable.React.ReactDomBindings.ReactDom.flushSync(ignore)
#endif
    match page with
    | Hello ->
        classP' "helloPage" Html.form [
            prop.children [
                class' "header" Html.span [
                    classTxt' "greeting" Html.div $"Hello! What's your name?"
                    classP' "settings" Html.button [prop.type'.button; prop.text $"Settings"]
                    ]

                Html.input [prop.type'.text; prop.valueOrDefault name; prop.onChange setName; prop.autoFocus true]
                Html.div [
                    Html.button [prop.className "startButton"; prop.type'.submit; prop.text "Start"]
                    ]
                ]
            prop.onSubmit (fun e ->
                e.preventDefault()
                setPage Main
                )
            ]
    | Main ->
        Main.Export.Component name (fun _ -> setPage Hello)
    | Settings ->
        Settings.Component()
    | HighScore ->
        HighScore.HighScores highScores (fun _ -> setPage Hello)
