module Elves

open System

open Domain

let parse lines =
    let elves = Elves ()
    
    let parseChar row (col, ch) =
        if ch = '#' then elves.Add {X = col; Y = row} |> ignore

    let parseLine (row, line) =
        line
        |> Seq.indexed
        |> Seq.iter (parseChar row)

    lines
    |> Seq.indexed
    |> Seq.iter parseLine

    elves

let frontTiles direction tile =
    match direction with
    | North ->
        let n = tile.Y - 1
        [| {X = tile.X - 1; Y = n}; {tile with Y = n}; {X = tile.X + 1; Y = n} |]
    | East ->
        let e = tile.X + 1
        [| {X = e; Y = tile.Y - 1}; {tile with X = e}; {X = e; Y = tile.Y + 1} |]
    | South ->
        let s = tile.Y + 1
        [| {X = tile.X - 1; Y = s}; {tile with Y = s}; {X = tile.X + 1; Y = s} |]
    | West ->
        let w = tile.X - 1
        [| {X = w; Y = tile.Y - 1}; {tile with X = w}; {X = w; Y = tile.Y + 1} |]

let neighbourTiles tile =
    let n, s, w, e = tile.Y - 1, tile.Y + 1, tile.X - 1, tile.X + 1
    [|
        {X = w; Y = n}; {tile with Y = n}; {X = e; Y = n};
        {tile with X = w};    (* Elf *)    {tile with X = e};
        {X = w; Y = s}; {tile with Y = s}; {X = e; Y = s}
    |]

let computeEmptyTiles (elves : Elves) =
    let computeMinMax (w, n, e, s) tile =
        min w tile.X, min n tile.Y, max e tile.X, max s tile.Y

    let initialState =
        Int32.MaxValue, Int32.MaxValue, Int32.MinValue, Int32.MinValue

    let w, n, e, s =
        elves
        |> Seq.fold computeMinMax initialState

    let minRectangleArea = (e - w + 1) * (s - n + 1)
    minRectangleArea - elves.Count
