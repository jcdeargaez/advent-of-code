open System.IO

type Range = {
    Begin : int
    End : int
}

type Pair = {
    Left : Range
    Right : Range
}

let fullyContained {Right = right; Left = left} =
    (left.Begin <= right.Begin && right.End <= left.End) || (right.Begin <= left.Begin && left.End <= right.End)

let overlapAtAll {Right = right; Left = left} =
    (left.Begin <= right.Begin && right.Begin <= left.End) || (right.Begin <= left.Begin && left.Begin <= right.End)

let count overlap pairs =
    pairs
    |> Seq.filter overlap
    |> Seq.length

let parseLine (line : string) =
    let pairs =
        line.Split ','
        |> Array.map (fun range ->
            match range.Split '-' with
            | [| b; e |] -> {Begin = int b; End = int e}
            | _ -> failwith "Invalid format")
    {Left = pairs.[0]; Right = pairs.[1]}

let pairs =
    "input.txt"
    |> File.ReadLines
    |> Seq.map parseLine
    |> Seq.toList

pairs
|> count fullyContained
|> printfn "Part 1: %i"

pairs
|> count overlapAtAll
|> printfn "Part 2: %i"
