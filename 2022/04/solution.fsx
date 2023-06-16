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

module Api =
    open System.IO
    
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

    let processInput path =
        let count (part1SoFar, part2SoFar) pair =
            let part1SoFar' = part1SoFar + if fullyContained pair then 1 else 0
            let part2SoFar' = part2SoFar + if overlapAtAll pair then 1 else 0
            part1SoFar', part2SoFar'

        path
        |> File.ReadLines
        |> Seq.map parseLine
        |> Seq.fold count (0, 0)

let part1, part2 = Api.processInput "input.txt"
printfn "Part 1: %i" part1
printfn "Part 2: %i" part2
