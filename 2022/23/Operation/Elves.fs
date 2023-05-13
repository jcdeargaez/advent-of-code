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

let inline frontTiles direction tile =
    [| direction.FrontLeft tile; direction.Front tile; direction.FrontRight tile |]

let inline neighbourTiles tile =
    [|
        Direction.northWest tile; Direction.north tile; Direction.northEast tile;
        Direction.west tile;          (* Elf *)         Direction.east tile;
        Direction.southWest tile; Direction.south tile; Direction.southEast tile
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
