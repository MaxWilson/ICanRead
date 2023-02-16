module Main

open Elmish
open Elmish.Navigation
open Elmish.React
open Home

Program.mkProgram init update view
    |> Program.toNavigable Nav.parse Nav.nav
    |> Program.withReactBatched "root"
    |> Program.run