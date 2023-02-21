module GameLogic

open GameData

type Problem = {
    words: string array
    answer: string
    }

type Feedback = Correct | Incorrect
type Game = {
    score: int
    problem: Problem
    reviewList: string list
    feedback: Feedback * string
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
        fresh(reviewList, 4)

let check problem word =
    problem.answer = word

let init _ =
    let game = { score = 0; problem = { words = [||]; answer = "" }; reviewList = []; feedback = Correct, "" }
    { game with problem = newProblem game }

let update game word =
    if check game.problem word then
        let game = { game with reviewList = game.reviewList |> List.filter ((<>) game.problem.answer) }
        { game with score = game.score + 100; problem = newProblem game; feedback = Correct, $"Correct!" }
    else
        let answer = game.problem.answer
        { game with score = game.score - 100; reviewList = word::answer::game.reviewList |> List.distinct |> List.sort; problem = newProblem game; feedback = Incorrect, $"No, that said '{word}', not '{answer}'." }