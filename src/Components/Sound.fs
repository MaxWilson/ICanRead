module Sound

open Fable.Core
open Fable.Core.JsInterop
open GameLogic

[<ImportAll("microsoft-cognitiveservices-speech-sdk")>]
let speech: obj = jsNative

[<Emit("new $0.SpeechSynthesizer($1, $2)")>]
let makeSpeechSynthesizer(sdk, speechConfig, audioConfig) = jsNative

let synthesizer(token) : obj =
    let speechConfig = speech?SpeechConfig?fromSubscription("526292c93472431687fbbd3bd8432fdd", "westus2")
    speechConfig?speechSynthesisVoiceName <- "en-US-MichelleNeural";
    let audioConfig = speech?AudioConfig?fromDefaultSpeakerOutput()
    makeSpeechSynthesizer(speech, speechConfig, audioConfig)

let speak (txt: string) =
    promise {
        synthesizer()?speakTextAsync(
            txt, ignore, fun (x:obj) -> System.Console.WriteLine("error", x))
    } |> ignore

let makeSound(id) =
    Promise.create(fun resolve reject ->
        let audio = Browser.Dom.document.getElementById(id)
        if audio?paused then
            audio?currentTime <- 0
            audio?onended <- fun _ -> resolve()
            audio?onerror <- fun e -> reject e
            audio?volume <- 0.4
            audio?controls <- false
            audio?loop <- false
            audio?play()
        )

let cheers =
    [for i in 1..5 do
        $"Cheer{i}"
        ]
let bomb = "Bomb"

let defaultName name =
    if String.isNullOrWhitespace name then "stranger" else name

let verbalizeProblem sound (game:Game) =
    match sound with
    | Verbose ->
        speak $" Which button says '{game.problem.answer}'?"
    | Terse | Effects ->
        speak game.problem.answer
let sayHello userName =
    speak $"Hello {userName |> defaultName}!"
let sayHelloAndVerbalizeProblem sound userName (game:Game) =
    match sound with
    | Verbose ->
        speak $"Hello {userName |> defaultName}! Can you show me which button says '{game.problem.answer}'?"
    | Terse | Effects ->
        speak game.problem.answer


