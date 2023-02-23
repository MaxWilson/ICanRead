module DataContracts

open System

[<AutoOpen>]
module HighScore =
    type Row = {
        name: string
        score: int
        date: DateTimeOffset
        }

    type DatelessRow = {
        name: string
        score: int
        }

    type HighScores = {
        bestEver: DatelessRow array
        bestRecent: DatelessRow array
        }
