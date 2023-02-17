module Main

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
    | SayHello
    | VerbalizeProblem
    | Pick of word: string
    | HelpLetter of letter: string

[<Emit("""rate => txt => {
    var msg = new SpeechSynthesisUtterance(txt);
    msg.pitch = 1.5;
    msg.rate = rate;
    msg.onerror = (event) => {
        console.log(`An error has occurred with the speech synthesis: ${event.error}`, event);
      };
    speechSynthesis.speak(msg)
}""")>]
let private speak : double -> string -> unit = jsNative

[<Emit("""rate => txt => {
    window.speechSynthesis.cancel();
    var msg = new SpeechSynthesisUtterance(txt);
    msg.pitch = 1.5;
    msg.rate = rate;
    msg.onerror = (event) => {
        console.log(`An error has occurred with the speech synthesis: ${event.error}`, event);
      };
    speechSynthesis.speak(msg)
}""")>]
let private interrupt : double -> string -> unit = jsNative

let update msg model =
    match msg with
    | Pick word ->
        let game = GameLogic.update model.game word
        interrupt 1. (game.feedback)
        speak 1. $" Now, which button says '{game.problem.answer}'?"
        { model with game = game }, []
    | HelpLetter letter ->
        interrupt 0.5 letter
        model, Cmd.Empty
    | VerbalizeProblem ->
        interrupt 1. $" Which button says '{model.game.problem.answer}'?"
        model, Cmd.Empty
    | SayHello ->
        interrupt 1.2 $"Hello {model.userName}. Can you show me which button says '{model.game.problem.answer}'?"
        model, Cmd.Empty

let init (userName: string) =
    {
        userName = if userName.Trim() = "" then "stranger" else userName
        game = GameLogic.init()
        }, []

let class' (className: string) ctor (elements: _ seq) = ctor [prop.className className; prop.children elements]
let classP' (className: string) ctor (props: IReactProperty list) = ctor (prop.className className::props)
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
                    classP' "guessButton" Html.button [
                        prop.text word
                        prop.onClick (fun _ -> dispatch (Pick word))
                        ]
                ]


            Html.button [prop.text "Say it again"; prop.onClick (thunk1 dispatch VerbalizeProblem)]
            if model.game.feedback <> "" then
                Html.div model.game.feedback

            if model.game.reviewList.Length > 0 then
                Html.div $"""Review list: {model.game.reviewList |> String.join ", " }"""
            ]
        ]
