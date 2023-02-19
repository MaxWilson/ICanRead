module HelloPage

open Elmish
open Elmish.Navigation
open Elmish.React
open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.UseElmish

[<ReactComponent>]
let HelloPage mainPage =
    let started, setStarted = React.useState false
    let name, setName = React.useState ""
    if not started then
        class' "helloPage" Html.form [
            Html.div "Hello! What's your name?"
            Html.input [prop.type'.text; prop.valueOrDefault name; prop.onChange setName; prop.autoFocus true]
            Html.div [
                Html.button [prop.className "startButton"; prop.type'.submit; prop.onClick (fun _ -> setStarted true); prop.text "Start"]
                ]
            ]
    else
        mainPage name (fun _ -> setStarted false)