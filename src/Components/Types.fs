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

module Settings =
    type Props = {
        onQuit: unit -> unit
        }

module HelloPage =
    type Props = {
        scores: HighScore.Row array * HighScore.Row array
        }