module Types
open System

type PageSelector = Hello | Main | Settings | HighScore

module HighScore =
    type Row = {
        name: string
        score: int
        date: DateTimeOffset
        }

module HelloPage =
    type Props = {
        scores: HighScore.Row array * HighScore.Row array
        }