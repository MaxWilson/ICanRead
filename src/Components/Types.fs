module Types
open System


// we put certain types in here so we can export only ReactComponents from the associated pages,
// and therefore not mess up hot-loading during development. (It's a limitation of React Fast-refresh.)
type PageSelector = Hello | Main | Settings | HighScore

module HighScore =
    type Row = {
        name: string
        score: int
        date: DateTimeOffset
        }
    type Props = {
        scores: Row array * Row array
        onQuit: (unit -> unit) option
        }

module Settings =
    type Settings = {
        currentSound: Sound
        setSound: Sound -> unit
        }
    type Props = {
        onQuit: (unit -> unit) option
        settings: Settings
        }

module Main =
    type Props = {
        userName: string
        scores: HighScore.Row array * HighScore.Row array
        settings: Settings.Settings
        }

module HelloPage =
    type Props = {
        scores: HighScore.Row array * HighScore.Row array
        settings: Settings.Settings
        }

