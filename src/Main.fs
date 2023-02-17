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

module private Impl =

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

    [<ImportAll("microsoft-cognitiveservices-speech-sdk")>]
    let speech: obj = jsNative

    [<Emit("new $0.SpeechSynthesizer($1, $2)")>]
    let makeSpeechSynthesizer(sdk, speechConfig, audioConfig) = jsNative

    let synthesizer : obj =
        let speechConfig = speech?SpeechConfig?fromSubscription("76cc92f037114e21b082bc53995e265f", "westus2")
        speechConfig?speechSynthesisVoiceName <- "en-US-MichelleNeural";
        let audioConfig = speech?AudioConfig?fromDefaultSpeakerOutput()
        makeSpeechSynthesizer(speech, speechConfig, audioConfig)

    let speak (txt: string) =
        synthesizer?speakTextAsync(
            txt, (fun (x:obj) -> System.Console.WriteLine x), fun (x:obj) -> System.Console.WriteLine x)

    let update msg model =
        match msg with
        | Pick word ->
            let game = GameLogic.update model.game word
            speak $"{game.feedback} Now, which button says '{game.problem.answer}'?"
            { model with game = game }, []
        | HelpLetter letter ->
            speak letter
            model, Cmd.Empty
        | VerbalizeProblem ->
            speak $" Which button says '{model.game.problem.answer}'?"
            model, Cmd.Empty
        | SayHello ->
            speak $"Hi {model.userName}! Can you show me which button says '{model.game.problem.answer}'?"
            model, Cmd.Empty

    let init (userName: string) =
        {
            userName = if userName.Trim() = "" then "stranger" else userName
            game = GameLogic.init()
            }, Cmd.ofMsg SayHello

    let navigateTo (url: string) =
        Browser.Dom.window.location.assign url

    let view model dispatch =
        class' "main" Html.div [
            classTxt' "userName" Html.div $"Hi, {model.userName}!"
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

module public Export =
    open Impl
    open Feliz.UseElmish

    [<ReactComponent>]
    let Component(name) =
        let model, dispatch = React.useElmish((fun _ -> Program.mkProgram init update (fun _ _ -> ())), arg=name)
        view model dispatch
