module Home

open Browser.Dom

open Feliz
open Feliz.Router
open Fable.Core.JsInterop

open Components
open Elmish
open Elmish.Navigation
open Thoth.Json
open GameLogic
open Fable.Core

importSideEffects "./styles.sass"

type 't Deferred = NotStarted | InProgress | Ready of 't

type Model = {
    userName: string
    game: Game
    }

type Msg =
    | Pick of word: string
    | HelpLetter of letter: string

[<Emit("""rate => msg => {
    var msg = new SpeechSynthesisUtterance(msg);
    msg.pitch = 28
    msg.rate = rate;
    window.speechSynthesis.speak(msg)
}""")>]
let private speak : double -> string -> unit = jsNative

let update msg model =
    match msg with
    | Pick word ->
        let game = GameLogic.update model.game word
        match game.problem.answer with
        | "" -> ()
        | v ->
            speak 1. (game.feedback + $" Now, find '{v}'.")
        { model with game = game }
    | HelpLetter letter ->
        speak 0.15 letter
        model

type NavCmd = Fresh

let init _ =
    let game = GameLogic.init()
    match game.problem.answer with
    | "" -> ()
    | v ->
        speak 1. ("Which word says " + v)
    {
        userName = "<UserName>"
        game = game
        }

let class' (className: string) ctor (elements: _ seq) = ctor [prop.className className; prop.children elements]
let classTxt' (className: string) ctor (txt: string) = ctor [prop.className className; prop.text txt]

let navigateTo (url: string) =
    Browser.Dom.window.location.assign url

let view model dispatch =
    class' "main" Html.div [
        classTxt' "userName" Html.div $"Hello, {model.userName}"
        classTxt' "score" Html.div $"Score: {model.game.score}"

        class' "guessing" Html.div [
            class' "choices" Html.section [
                for word in model.game.problem.words do
                    class' "guessWord" Html.div [
                        for letter in word do
                            Html.span [
                                prop.className "guessLetter"
                                prop.text (letter.ToString())
                                prop.onClick (fun _ -> dispatch (HelpLetter (letter.ToString())))
                                ]
                        ]
                    Html.button [
                        prop.text word
                        prop.onClick (fun _ -> dispatch (Pick word))
                        ]
                ]

            if model.game.feedback <> "" then
                Html.div model.game.feedback

            if model.game.reviewList.Length > 0 then
                Html.div $"""Review list: {model.game.reviewList |> String.join ", " }"""
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
            Fresh
        | _ -> Fresh
    let nav (navCmd: NavCmd) model =
        init navCmd, Cmd.Empty

open Elmish.React
