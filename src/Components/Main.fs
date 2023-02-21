module Main

open Browser.Dom

open Feliz
open Feliz.Router
open Fable.Core.JsInterop

open Elmish
open Elmish.Navigation
open Thoth.Json
open GameLogic
open Fable.Core

module private Impl =
    open Types

    type 't Deferred = NotStarted | InProgress | Ready of 't
    type Modal = Settings | HighScore
    type Model = {
        userName: string
        game: Game
        openDialog: Modal option
        }

    type Msg =
        | SayHello
        | SayHelloAndVerbalizeProblem
        | VerbalizeProblem
        | Pick of word: string
        | HelpLetter of letter: string
        | SetDialog of Modal option

    [<ImportAll("microsoft-cognitiveservices-speech-sdk")>]
    let speech: obj = jsNative

    [<Emit("new $0.SpeechSynthesizer($1, $2)")>]
    let makeSpeechSynthesizer(sdk, speechConfig, audioConfig) = jsNative

    let synthesizer(token) : obj =
        let resourceId = "/subscriptions/2f4e9444-9448-4618-b9e4-4d41ce647375/resourceGroups/ShiningSword/providers/Microsoft.CognitiveServices/accounts/shiningswordspeech"
        let speechConfig = speech?SpeechConfig?fromAuthorizationToken($"aad#{resourceId}#{token}", "westus2")
        speechConfig?speechSynthesisVoiceName <- "en-US-MichelleNeural";
        let audioConfig = speech?AudioConfig?fromDefaultSpeakerOutput()
        makeSpeechSynthesizer(speech, speechConfig, audioConfig)

    let speak (token:string) (txt: string) =
        promise {
            synthesizer(token)?speakTextAsync(
                txt, ignore, fun (x:obj) -> System.Console.WriteLine("error", x))
        } |> Promise.start

    let makeSound(url) =
        Promise.create(fun resolve reject ->
            let audio = Browser.Dom.document.createElement("audio")
            audio?volume <- 0.4
            audio?src <- url
            audio?autoplay <- true
            audio?controls <- false
            audio?loop <- false
            audio?onended <- fun _ -> resolve(); audio?remove()
            audio?onerror <- fun e -> reject e
            )

    let cheers =
        [
            "Cheer1.m4a"
            "Cheer2.m4a"
            "Cheer4.m4a"
            "Cheer5.m4a"
            "1_person_cheering-Jett_Rifkin-1851518140.mp3"
            ] |> List.map (sprintf "../assets/%s")

    let bomb = "../assets/Grenade Explosion-SoundBible.com-2100581469.mp3"

    let update (speak, sound: Sound IRefValue) msg model =
        match msg with
        | Pick word ->
            let game = GameLogic.update model.game word
            match sound.current with
            | Verbose ->
                speak $"{snd game.feedback} Now, which button says '{game.problem.answer}'?"
            | Terse ->
                speak game.problem.answer
            | Effects ->
                promise {
                    speak game.problem.answer
                    do! makeSound (if game.feedback |> fst = Correct then List.chooseRandom cheers else bomb)
                    } |> Promise.start
            { model with game = game }, []
        | HelpLetter letter ->
            speak letter
            model, Cmd.Empty
        | VerbalizeProblem ->
            match sound.current with
            | Verbose ->
                speak $" Which button says '{model.game.problem.answer}'?"
            | Terse | Effects ->
                speak model.game.problem.answer
            model, Cmd.Empty
        | SayHello ->
            speak $"Hello {model.userName}!"
            model, Cmd.Empty
        | SayHelloAndVerbalizeProblem ->
            match sound.current with
            | Verbose ->
                speak $"Hello {model.userName}! Can you show me which button says '{model.game.problem.answer}'?"
            | Terse | Effects ->
                speak model.game.problem.answer
            model, Cmd.Empty
        | SetDialog v ->
            { model with openDialog = v }, Cmd.Empty

    let init (userName: string) =
        {
            userName = if userName.Trim() = "" then "stranger" else userName
            game = GameLogic.init()
            openDialog = None
            }, Cmd.ofMsg SayHelloAndVerbalizeProblem

    let navigateTo (url: string) =
        Browser.Dom.window.location.assign url

    let view model onQuit dispatch =
        class' "main" Html.div [
            class' "header" Html.span [
                classP' "userName" Html.div [prop.text $"Hello, {model.userName}!"; prop.onClick (thunk1 dispatch SayHello)]
                classP' "settings" Html.button [prop.onClick (thunk1 dispatch (SetDialog (Some Settings))); prop.text $"Settings"]
                classP' "highscores" Html.button [prop.onClick (thunk1 dispatch (SetDialog (Some HighScore))); prop.text $"High scores"]
                classP' "score" Html.span [prop.onClick (thunk1 dispatch (SetDialog (Some HighScore))); prop.text $"Score: {model.game.score}"]
                classP' "quit" Html.button [prop.text $"Quit"; prop.onClick (thunk1 onQuit ())]
                ]

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

                classP' "againButton" Html.button [prop.text "Say it again"; prop.onClick (thunk1 dispatch VerbalizeProblem)]
                let feedback = model.game.feedback |> snd
                if feedback <> "" then
                    Html.div feedback

                if model.game.reviewList.Length > 0 then
                    Html.div $"""Review list: {model.game.reviewList |> String.join ", " }"""
                ]
            ]

open Impl
open Feliz.UseElmish

[<ReactComponent>]
let Component (props: Types.Main.Props) onQuit =
    let name = props.userName
    let sound = React.useRef props.settings.currentSound
    sound.current <- props.settings.currentSound
    let model, dispatch = React.useElmish((fun _ -> Program.mkProgram init (update (speak props.speechToken, sound)) (fun _ _ -> ())), arg=name)
    match model.openDialog with
    | None ->
        view model onQuit dispatch
    | Some Settings ->
        Settings.Component { onQuit = Some (thunk1 dispatch (SetDialog None)); settings = props.settings }
    | Some HighScore ->
        HighScore.Component { scores = props.scores; onQuit = Some (thunk1 dispatch (SetDialog None)) }
