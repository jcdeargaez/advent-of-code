open System.Diagnostics
open System.IO

let priority item =
    int item - (if item <= 'Z' then int '@' - 26 else int '`')

let commonItemsPriority containers =
    containers
    |> Seq.map Set
    |> Seq.reduce (fun acc s -> acc |> Set.intersect s)
    |> Seq.sumBy priority

let part1 lines =
    lines
    |> Seq.sumBy (fun line ->
        let mid = String.length line / 2
        commonItemsPriority [line.[..mid - 1]; line.[mid..]])

let part2 lines =
    lines
    |> Seq.chunkBySize 3
    |> Seq.sumBy commonItemsPriority

let start = Stopwatch.StartNew ()
let content = File.ReadLines "input.txt"
printfn "Part 1: %i" (part1 content)
printfn "Part 2: %i" (part2 content)
printfn "%.5fms" start.Elapsed.TotalMilliseconds
