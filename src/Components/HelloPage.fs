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
    let started, setStarted = React.useState false
    let backToHello = (thunk1 setStarted false)
    if not started then
        class' "helloPage" Html.div [
            class' "header" Html.span [
                classTxt' "greeting" Html.div $"Hello! What's your name?"
                ]

            classP' "inputArea" Html.form [
                prop.children [
                    Html.input [prop.type'.text; prop.valueOrDefault name; prop.onChange setName; prop.autoFocus true]
                    Html.button [prop.className "startButton"; prop.type'.submit; prop.text "Start"]
                    ]
                prop.onSubmit (fun e ->
                    e.preventDefault()
                    setStarted true
                    )
                ]
            Settings.Component { settings = props.settings; onQuit = None }
            HighScore.Component { scores = props.scores; onQuit = None }
            ]
    else
        Main.Component { userName = name; scores = props.scores; settings = props.settings } backToHello
