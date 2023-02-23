module DataContracts

open System

[<AutoOpen>]
module HighScore =
    type Row = {
        name: string
        score: int
        }

    type HighScores = {
        allTime: Row array
        recent: Row array
        }
