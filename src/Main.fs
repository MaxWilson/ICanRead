module Main

open Elmish
open Elmish.Navigation
open Elmish.React
open Home

Program.mkSimple init update view
    |> Program.toNavigable Nav.parse Nav.nav
    |> Program.withReactBatched "root"
    |> Program.run