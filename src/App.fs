module App

open Elmish
open Elmish.Navigation
open Elmish.React
open Fable.Core
open Fable.Core.JsInterop

importSideEffects "./styles.sass"

let root = Feliz.ReactDOM.createRoot(Browser.Dom.document.getElementById "root")
root.render(HelloPage.HelloPage(Main.Export.Component))

