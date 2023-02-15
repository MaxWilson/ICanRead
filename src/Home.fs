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

importSideEffects "./styles.sass"

type 't Deferred = NotStarted | InProgress | Ready of 't

type Model = {
    userName: string
    game: Game
    help: string option
    }

type Msg =
    | Pick of word: string
    | HelpLetter of letter: string

let update msg model =
    match msg with
    | Pick word ->
        { model with game = GameLogic.update model.game word; help = None }
    | HelpLetter letter ->
        { model with help = Some letter }

type NavCmd = Fresh

let init _ =
    {
        userName = "<UserName>"
        game = GameLogic.init()
        help = None
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
            classTxt' "speakingVoice" Html.div $"<<A voice says '{model.game.problem.answer}'>>"

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
            Html.div model.game.feedback

            if model.game.reviewList.Length > 0 then
                Html.div $"""Review list: {model.game.reviewList |> String.join ", " }"""
            ]

        match model.help with
        | None -> ()
        | Some letter ->
            classTxt' "help" Html.div $"The letter '{letter}' sounds like <<A voice says '{letter}'>>"
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
