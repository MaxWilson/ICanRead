[<AutoOpen>]
module Common
open System.Threading.Tasks

let thunk v _ = v
let thunk1 f x _ = f x
let thunk2 f x y _ = f x y
let thunk3 f x y z _ = f x y z
let ignore2 _ _ = ()
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
    let every pred arr =
        arr |> Array.exists (not << pred) |> not
module String =
    let join (sep:string) (input: string seq) = System.String.Join(sep, input)
    let isNullOrWhitespace (s: string) = System.String.IsNullOrWhiteSpace s
module List =
    let mapiOneBased f lst = lst |> List.mapi (fun ix -> f (ix + 1))
    let create v = [v]
    let chooseRandom (lst: _ list) =
        lst[rand.Next lst.Length]
    let every pred arr =
        arr |> List.exists (not << pred) |> not

#if FABLE_COMPILER
#else
module Task =
    let map (f: 't -> 'r) (t: 't Task) = task { let! result = t in return f result }
    let ignore (t: Task<'a>) = t :> Task
#endif

let inline trace label x =
#if DEBUG
    printfn $"Trace/{label}: {x}"
#endif
    x

let inline log element = System.Console.WriteLine(element |> box)