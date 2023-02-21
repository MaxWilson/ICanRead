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

let createAudio id =
    if Browser.Dom.document.getElementById id |> isNull then
        let audio = Browser.Dom.document.createElement("audio")
        audio?hidden <- true
        audio?volume <- 0.4
        audio?src <- $"assets/{id}.m4a"
        audio?id <- id
        audio?controls <- false
        Browser.Dom.document.head.appendChild(audio) |> ignore
        audio?load()

for id in ["Cheer1";"Cheer2";"Cheer3";"Cheer4";"Cheer5";"Bomb"] do
    createAudio id

