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
    | VerbalizeProblem
    | Interrupt
    | Pick of word: string
    | HelpLetter of letter: string

[<Emit("""rate => msg => {
    var msg = new SpeechSynthesisUtterance(msg);
    msg.pitch = 3
    msg.rate = rate;
    window.speechSynthesis.speak(msg)
}""")>]
let private speak : double -> string -> unit = jsNative

[<Emit("""rate => msg => {
    window.speechSynthesis.cancel();
    var msg = new SpeechSynthesisUtterance(msg);
    msg.pitch = 3
    msg.rate = rate;
    window.speechSynthesis.speak(msg)
}""")>]
let private interrupt : double -> string -> unit = jsNative

let update msg model =
    match msg with
    | Pick word ->
        let game = GameLogic.update model.game word
        speak 1. (game.feedback)
        speak 1. $" Now, which button says '{game.problem.answer}'?"
        { model with game = game }, []
    | HelpLetter letter ->
        interrupt 0.5 letter
        model, Cmd.Empty
    | VerbalizeProblem ->
        match model.game.problem.answer with
        | "" -> ()
        | v ->
            interrupt 1. $" Which button says '{v}'?"
        model, Cmd.Empty

type NavCmd = Fresh

let init _ =
    {
        userName = "<UserName>"
        game = GameLogic.init()
        }, Cmd.ofMsg VerbalizeProblem

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


            Html.button [prop.text "Say it again"; prop.onClick (thunk1 dispatch VerbalizeProblem)]
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
        init navCmd
