[<AutoOpen>]
module Common

let thunk v _ = v
let thunk1 f x _ = f x
let thunk2 f x y = f x y
let notImpl msg = failwith $"Not implemented! Talk to Max if you want this feature. {msg}"
let shouldntHappen data =
    System.Console.WriteLine(data :> obj)
    failwith $"This shouldn't happen. There must be a bug. {data}"
let inline breakHere() = System.Diagnostics.Debugger.Break()
let tuple2 x y = x,y
let flip f x y = f y x
let rand = System.Random()
module Array =
    let chooseRandom (lst: _ array) =
        lst[rand.Next lst.Length]
    let randomize (lst: _ array) =
        // copilot-generated function so it's not exactly idiomatic, but looks correct enough
        let lst = lst |> Array.copy
        let mutable i = lst.Length
        while i > 1 do
            let j = rand.Next(i)
            i <- i - 1
            let tmp = lst.[i]
            lst.[i] <- lst.[j]
            lst.[j] <- tmp
        lst
module String =
    let join (sep:string) (input: string seq) = System.String.Join(sep, input)
module List =
    let mapiOneBased f lst = lst |> List.mapi (fun ix -> f (ix + 1))
    let create v = [v]
    let chooseRandom (lst: _ list) =
        lst[rand.Next lst.Length]

let inline trace label x =
#if DEBUG
    printfn $"Trace/{label}: {x}"
#endif
    x