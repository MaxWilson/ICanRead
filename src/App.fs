module App

open Elmish
open Elmish.Navigation
open Elmish.React
open Fable.Core
open Fable.Core.JsInterop
open Types.HighScore

importSideEffects "./styles.sass"
let highScores =
    [|
        { name = "Therion"; score = 1000; date = System.DateTimeOffset.Now }
        |],
    [|
        { name = "Therion"; score = 1000; date = System.DateTimeOffset.Now }
        |]
let x() = HelloPage.HelloPage { scores = highScores }
let root = Feliz.ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(HelloPage.HelloPage { scores = highScores })

