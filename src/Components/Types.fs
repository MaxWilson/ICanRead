module Types
open System
open DataContracts

// we put certain types in here so we can export only ReactComponents from the associated pages,
// and therefore not mess up hot-loading during development. (It's a limitation of React Fast-refresh.)
type PageSelector = Hello | Main | Settings | HighScore

module HighScore =
    type Props = {
        onQuit: (unit -> unit) option
        registerForUpdates: bool
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
        settings: Settings.Settings
        }

module HelloPage =
    type Props = {
        settings: Settings.Settings
        }

