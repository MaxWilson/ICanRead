module App

open Elmish
open Fable.Core.JsInterop
open Types.HighScore
open Types
open Feliz

importSideEffects "./styles.sass"
let highScores =
    [|
        { name = "Therion"; score = 1000; date = System.DateTimeOffset.Now }
        |],
    [|
        { name = "Therion"; score = 1000; date = System.DateTimeOffset.Now }
        |]
let root = Feliz.ReactDOM.createRoot(Browser.Dom.document.getElementById "root")

[<Feliz.ReactComponent>]
let Root() =
    let sound, setSound = React.useState (LocalStorage.Sound.read())
    let setSound v =
        LocalStorage.Sound.write v
        setSound v
    HelloPage.HelloPage { scores = highScores; settings = { currentSound = sound; setSound = setSound } }

root.render(Root())

