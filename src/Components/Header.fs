module Header
open Feliz
open UICommon

let header (prefixElements, score, onQuit) =
    class' "header" Html.span [
        yield! prefixElements
        classTxt' "settings" Html.button $"Settings"
        classTxt' "highscores" Html.button $"High scores"
        classTxt' "score" Html.span $"Score: {score}"
        classP' "quit" Html.button [prop.text $"Quit"; prop.onClick (thunk1 onQuit ())]
        ]