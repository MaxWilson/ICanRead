module Settings
open Feliz
open Feliz.UseElmish
open Elmish

module private Impl =
    type Model = { data: unit }
    let init _ = { data = () }
    type Msg = Msg
    let update msg model =
        match msg with
        | Msg ->
            model
    let view model dispatch =
        class' "settings" Html.div [

            ]



open Impl
let Component() =
    let model, dispatch = React.useElmish(thunk3 Program.mkSimple init update ignore2)
    view model dispatch
