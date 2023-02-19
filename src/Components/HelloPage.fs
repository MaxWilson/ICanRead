module HelloPage

open Elmish
open Elmish.Navigation
open Elmish.React
open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.UseElmish
open Browser.Types

[<ReactComponent>]
let HelloPage mainPage =
    let started, setStarted = React.useState false
    let name, setName = React.useState ""
#if DEBUG
    // workaround for strange issue in dev where state updates don't get flushed/UI doesn't update after setState. E.g. clicking the Start button does nothing until you open the Chrome dev console.
    let setStarted x =
        setStarted x
        Fable.React.ReactDomBindings.ReactDom.flushSync(ignore)
#endif

    if not started then
        classP' "helloPage" Html.form [
            prop.children [
                Html.div $"Hello! What's your name?"
                Html.input [prop.type'.text; prop.valueOrDefault name; prop.onChange setName; prop.autoFocus true]
                Html.div [
                    Html.button [prop.className "startButton"; prop.type'.submit; prop.text "Start"]
                    ]
                ]
            prop.onSubmit (fun e ->
                e.preventDefault()
                setStarted true
                )
            ]
    else
        mainPage name (fun _ -> setStarted false)