[<AutoOpen>]
module UICommon
open Feliz
open Fable.Core

type 't Deferred = NotStarted | InProgress | Ready of 't
type Sound = Verbose | Effects | Terse

let class' (className: string) ctor (elements: _ seq) = ctor [prop.className className; prop.children elements]
let classP' (className: string) ctor (props: IReactProperty list) = ctor (prop.className className::props)
let classTxt' (className: string) ctor (txt: string) = ctor [prop.className className; prop.text txt]
