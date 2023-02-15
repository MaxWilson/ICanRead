module Home

open Browser.Dom

open Feliz
open Feliz.Router
open Fable.Core.JsInterop

open Components
open Elmish
open Elmish.Navigation
open Thoth.Json

importSideEffects "./styles.sass"

type 't Deferred = NotStarted | InProgress | Ready of 't

type Model = {
    msg: string
    }

type Msg =
    | SetMsg of string

let update msg model =
    match msg with
    | SetMsg v -> { model with msg = v }

type NavCmd = Fresh | StartWith of string

let init = function
    | Fresh ->
        { msg = "Hello world" }
    | StartWith msg ->
        { msg = msg }

let class' (className: string) ctor (elements: _ seq) = ctor [prop.className className; prop.children elements]

let navigateTo (url: string) =
    Browser.Dom.window.location.assign url

let view model dispatch =
    Html.div [
        prop.className "container"
        prop.children [
            Html.h1 [prop.text model.msg]
            Html.button [
                prop.text "Get message"
                prop.onClick (fun _ -> dispatch (SetMsg ("Hello world!" + System.Guid.NewGuid().ToString())))
            ]
            Html.button [
                prop.text "Update URL"
                prop.onClick (fun _ -> navigateTo ("#hi stranger"))
            ]
        ]
    ]

module Nav =
    open Elmish.Navigation
    open Fable.Core

    [<Emit("decodeURIComponent($0)")>]
    let unescape arg = jsNative

    let parse (loc:Browser.Types.Location) : NavCmd =
        match loc.hash with
        | s when s.StartsWith("#") ->
            let msg = s.Substring(1).Replace("/", "") |> unescape
            msg |> StartWith
        | _ -> Fresh
    let nav (navCmd: NavCmd) model =
        init navCmd, Cmd.Empty

open Elmish.React
