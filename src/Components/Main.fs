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
    open Sound

    type 't Deferred = NotStarted | InProgress | Ready of 't
    type Modal = Settings | HighScore
    type Model = {
        userName: string
        game: Game
        openDialog: Modal option
        }

    type Msg =
        | Picked of Game
        | SetDialog of Modal option

    let pick (sound: Sound) word (model: Model) dispatch =
        let game = GameLogic.update model.game word
        match sound, fst game.feedback with
        | Verbose, _ ->
            speak $"{snd game.feedback} Now, which button says '{game.problem.answer}'?"
        | Terse, _ ->
            speak game.problem.answer
        | Effects, Incorrect ->
            makeSound bomb |> ignore
            speak game.problem.answer
        | Effects, Correct ->
            makeSound (List.chooseRandom cheers) |> ignore
            speak game.problem.answer
            HighScores.writeScore (model.userName, game.score)
        dispatch (Picked game)

    let update (sound: Sound IRefValue) msg model =
        match msg with
        | Picked game ->
            { model with game = game }
        | SetDialog v ->
            { model with openDialog = v }

    let init (userName: string, game: Game) =
        {
            userName = userName
            game = game
            openDialog = None
            }

    let navigateTo (url: string) =
        Browser.Dom.window.location.assign url

    let view model (sound, onQuit) dispatch =
        class' "main" Html.div [
            class' "header" Html.span [
                classP' "userName" Html.div [prop.text $"Hello, {model.userName |> defaultName}!"; prop.onClick (thunk1 sayHello model.userName)]
                classP' "settings" Html.button [prop.onClick (thunk1 dispatch (SetDialog (Some Settings))); prop.text $"Settings"]
                classP' "highscores" Html.button [prop.onClick (thunk1 dispatch (SetDialog (Some HighScore))); prop.text $"High scores"]
                classP' "score" Html.span [prop.onClick (thunk1 dispatch (SetDialog (Some HighScore))); prop.text $"Score: {model.game.score}"]
                classP' "quit" Html.button [prop.text $"Quit"; prop.onClick (thunk1 onQuit model)]
                ]

            class' "guessing" Html.div [
                class' "choices" Html.section [
                    for word in model.game.problem.words do
                        class' "guessWord" Html.div [
                            for letter in word do
                                Html.span [
                                    prop.className "guessLetter"
                                    prop.text (letter.ToString())
                                    prop.onClick (fun _ -> speak (letter.ToString()))
                                    ]
                            ]
                        classP' "guessButton" Html.button [
                            prop.text word
                            prop.onClick (fun _ -> pick sound word model dispatch)
                            ]
                    ]

                classP' "againButton" Html.button [prop.text "Say it again"; prop.onClick (thunk2 verbalizeProblem sound model.game)]
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
    let sound = React.useRef props.settings.currentSound
    sound.current <- props.settings.currentSound
    let writeToDbAndQuit (model: Model) =
        let row : DataContracts.HighScore.Row = { name = props.userName; score = model.game.score }
        Thoth.Fetch.Fetch.post<_, unit>("api/WriteScore", row) |> ignore // attempt to write but don't wait to see results
        onQuit()
    let model, dispatch = React.useElmish((fun _ -> Program.mkSimple init (update sound) (fun _ _ -> ())), arg=(props.userName,props.initialGame))
    match model.openDialog with
    | None ->
        view model (sound.current, writeToDbAndQuit) dispatch
    | Some Settings ->
        Settings.Component { onQuit = Some (thunk1 dispatch (SetDialog None)); settings = props.settings }
    | Some HighScore ->
        HighScore.Component { registerForUpdates = false; onQuit = Some (thunk1 dispatch (SetDialog None)) }
