module Settings
open Feliz
open Feliz.UseElmish
open Elmish
open Types
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
    let sound = props.settings.currentSound
    let setSound = props.settings.setSound
    class' "settings" Html.div [
        Html.div [
            Html.div "Sound:"
            for option in [Verbose; Effects; Terse] do
                let chkId = "chk" + option.ToString()
                Html.input [prop.type'.checkbox; prop.id chkId; prop.readOnly true; prop.isChecked ((option = sound)); prop.onClick(thunk1 setSound option)]
                Html.label [prop.htmlFor chkId; prop.text $"{option}"]
            ]
        match props.onQuit with
        | None -> ()
        | Some quit ->
            Html.div [
                Html.button [prop.text "OK"; prop.onClick (thunk1 quit ())]
                ]
        ]
