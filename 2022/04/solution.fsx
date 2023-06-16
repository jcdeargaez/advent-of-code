open System.IO

module Domain =
    type Range = {
        Begin : int
        End : int
    }

    type Pair = {
        Left : Range
        Right : Range
    }

module Range =
    open Domain

    let create b e =
        if e < b then invalidArg (nameof e) "e is lower than b"
        {Begin = b; End = e}

module Operations =
    open Domain

    let fullyContained {Right = right; Left = left} =
        (left.Begin <= right.Begin && right.End <= left.End) || (right.Begin <= left.Begin && left.End <= right.End)

    let overlapAtAll {Right = right; Left = left} =
        (left.Begin <= right.Begin && right.Begin <= left.End) || (right.Begin <= left.Begin && left.Begin <= right.End)

    let count overlap pairs =
        pairs
        |> Seq.filter overlap
        |> Seq.length

module Api =
    open Domain
    open Operations

    let parseLine (line : string) =
        let pairs =
            line.Split ','
            |> Array.map (fun range ->
                match range.Split '-' with
                | [| b; e |] -> Range.create (int b) (int e)
                | _ -> failwith "Invalid format")
        {Left = pairs.[0]; Right = pairs.[1]}

    let readInput path =
        path
        |> File.ReadLines
        |> Seq.map parseLine
        |> Seq.toList

    let part1 = count fullyContained
    let part2 = count overlapAtAll

let pairs = Api.readInput "input.txt"

Api.part1 pairs
|> printfn "Part 1: %i"

Api.part2 pairs
|> printfn "Part 2: %i"
