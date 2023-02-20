module Settings
open Feliz
open Feliz.UseElmish
open Elmish
open Types.Settings

[<AutoOpen>]
module private Impl =
    type Model = { data: unit }
    let init _ = { data = () }
    type Msg = Msg
    let update msg model =
        match msg with
        | Msg ->
            model

[<ReactComponent>]
let Component (props: Props) =
    let model, dispatch = React.useElmish(thunk3 Program.mkSimple init update ignore2)
    class' "settings" Html.div [
        Html.div "Placeholder"
        Html.button [prop.onClick (thunk1 props.onQuit ()); prop.text "Done"]
        ]
