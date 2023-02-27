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
    let game, setGame = React.useState None
    let backToHello = (thunk1 setGame None)
    match game with
    | None ->
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
                    let game = GameLogic.init()
                    Sound.sayHelloAndVerbalizeProblem props.settings.currentSound name game
                    setGame (Some game)
                    )
                ]
            Settings.Component { settings = props.settings; onQuit = None }
            HighScore.Component { registerForUpdates = true; onQuit = None }
            ]
    | Some game ->
        Main.Component { userName = name; settings = props.settings; initialGame = game } backToHello
