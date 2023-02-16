module HelloPage

open Elmish
open Elmish.Navigation
open Elmish.React
open Main
open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.UseElmish

[<ReactComponent>]
let HelloPage() =
    let started, setStarted = React.useState false
    let name, setName = React.useState ""
    let model, dispatch = React.useElmish((fun _ -> Program.mkProgram init update (fun _ _ -> ())), arg=name)
    if not started then
        class' "helloPage" Html.form [
            Html.div "Hello! What's your name?"
            Html.input [prop.type'.text; prop.valueOrDefault name; prop.onChange setName; prop.autoFocus true]
            Html.div [
                Html.button [prop.className "startButton"; prop.type'.submit; prop.onClick (fun _ -> setStarted true; dispatch SayHello); prop.text "Start"]
                ]
            ]
    else
        view model dispatch