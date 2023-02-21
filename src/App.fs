module App

open Elmish
open Fable.Core.JsInterop
open Types.HighScore
open Types
open Feliz
open DataContracts
importSideEffects "./styles.sass"

let highScores =
    [|
        { name = "Therion"; score = 3000; date = System.DateTimeOffset.Now }
        { name = "Jerry"; score = 2000; date = System.DateTimeOffset.Now }
        { name = "Mutt"; score = 1000; date = System.DateTimeOffset.Now }
        { name = "Tim"; score = 4000; date = System.DateTimeOffset.Now }
        |],
    [|
        { name = "Therion"; score = 3000; date = System.DateTimeOffset.Now }
        { name = "Tim"; score = 400; date = System.DateTimeOffset.Now }
        |]

[<Feliz.ReactComponent>]
let Root() =
    let sound, setSound = React.useState (LocalStorage.Sound.read())
    let setSound v =
        LocalStorage.Sound.write v
        setSound v
    HelloPage.HelloPage { scores = highScores; settings = { currentSound = sound; setSound = setSound } }

let root = Feliz.ReactDOM.createRoot(Browser.Dom.document.getElementById "root")

root.render(Root())

