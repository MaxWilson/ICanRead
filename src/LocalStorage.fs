﻿module LocalStorage
open Thoth.Json
open Browser.Dom
open UICommon

let inline jsonParse<'t> fallback str : 't =
    match Decode.Auto.fromString str with
    | Ok result -> result
    | Result.Error err ->
        fallback

let inline read (key: string) fallback =
    try
        Browser.Dom.window.localStorage[key] |> jsonParse<'t> fallback
    with _ ->
        fallback
let inline write (key: string) value =
    Browser.Dom.window.localStorage[key] <- Encode.Auto.toString<'t>(0, value)

module Cache =
    let create<'t>() =
        let mutable cache = None
        let read onCacheMiss arg =
            match cache with
            | Some v -> v
            | None -> onCacheMiss arg
        let invalidate() =
            cache <- None
        read, invalidate

open Cache

module Sound =
    let key = "Sound"
    let cacheRead, cacheInvalidate = Cache.create()
    let read (): Sound =
        cacheRead (fun _ -> read key Verbose) ()
    let write (v: Sound) =
        write key v
        cacheInvalidate()
