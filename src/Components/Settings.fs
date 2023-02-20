module Settings
open Feliz
open Feliz.UseElmish
open Elmish

[<AutoOpen>]
module private Impl =
    type Model = { data: unit }
    let init _ = { data = () }
    type Msg = Msg
    let update msg model =
        match msg with
        | Msg ->
            model

let Component () onQuit =
    let model, dispatch = React.useElmish(thunk3 Program.mkSimple init update ignore2)
    class' "settings" Html.div [
        Html.div "Placeholder"
        Html.button [prop.onClick (thunk1 onQuit ()); prop.text "Done"]
        ]
