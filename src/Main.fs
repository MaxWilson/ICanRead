module Main

open Elmish
open Elmish.Navigation
open Elmish.React
open Home

module Cmd =
    let ofSub f =
        [[], fun dispatch ->
                f dispatch
                { new System.IDisposable with
                               member this.Dispose(): unit = ()
                               }
            ]

Program.mkSimple init update view
    |> Program.toNavigable Nav.parse Nav.nav
    |> Program.withReactBatched "root"
    |> Program.run