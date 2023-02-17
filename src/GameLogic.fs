module GameLogic

open GameData

type Problem = {
    words: string array
    answer: string
    }

type Game = {
    score: int
    problem: Problem
    reviewList: string list
    feedback: string
    }

let newProblem(game:Game) =
    let fresh(seedWords, len) =
        let rec recur (accum: string list) =
            if accum.Length = len then accum
            else
                recur (((Array.chooseRandom words)::accum) |> List.distinct)
        let words = recur seedWords |> List.toArray |> Array.randomize
        // don't ask the same question twice in a row
        { words = words; answer = Array.chooseRandom (words |> Array.filter ((<>) game.problem.answer)) }
    match game.reviewList with
    | [] -> fresh([], 4)
    | _ ->
        let reviewList = game.reviewList |> List.take (min game.reviewList.Length 3) |> Array.ofList |> Array.randomize |> List.ofArray
        fresh([game.reviewList |> List.chooseRandom], 4)

let check problem word =
    problem.answer = word

let init _ =
    let game = { score = 0; problem = { words = [||]; answer = "" }; reviewList = []; feedback = "" }
    { game with problem = newProblem game }

let update game word =
    if check game.problem word then
        { game with score = game.score + 100; reviewList = game.reviewList |> List.filter ((<>) game.problem.answer); problem = newProblem game; feedback = $"Correct!" }
    else
        let answer = game.problem.answer
        { game with score = game.score - 100; reviewList = answer::game.reviewList |> List.distinct |> List.sort; problem = newProblem game; feedback = $"No, that said '{word}'." }