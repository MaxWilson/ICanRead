module Components
open Feliz
open Fetch
open Thoth.Fetch

type 't Deferred = NotStarted | InProgress | Ready of 't
type Point = { x: float; y: float }
type Stroke = { points: float array } // flattened coordinate list, e.g. [x1;y1;x2;y2] and so on. Perf optimization relative to flattening with every render.
type GraphicElement =
    | Stroke of Stroke * color: string * brushSize: string
    | Text of string * Point * color: string
