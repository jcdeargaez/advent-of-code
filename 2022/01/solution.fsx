open System.Diagnostics
open System.IO

let timeit logger f =
    let sw = Stopwatch.StartNew ()
    let r = f ()
    sw.Stop ()
    logger sw.Elapsed.TotalMilliseconds r

let solveFunctional topN input =
    let calories (i, caloriesSoFar) line =
        if line = "" then i + 1, 0
        else i, caloriesSoFar + int line
    
    let topAsc topAscSoFar calories =
        let i =
            topAscSoFar
            |> Array.indexed
            |> Array.takeWhile (snd >> (>) calories)
            |> Array.tryLast
            |> Option.map (fst >> (+) 1)
            |> Option.defaultValue 0
        topAscSoFar
        |> Array.insertAt i calories
        |> Array.skip (topAscSoFar.Length + 1 - topN)

    input
    |> Seq.scan calories (1, 0)
    |> Seq.groupBy fst
    |> Seq.map (snd >> Seq.last >> snd)
    |> Seq.fold topAsc Array.empty
    |> Array.sum

let solveImperative topN input =
    let mutable i, calories, topAsc = 1, 0, ResizeArray<int> (topN + 1)

    let updateTop v =
        let mutable i = 0
        while i < topAsc.Count && v > topAsc.[i] do
            i <- i + 1
        if i > 0 || topAsc.Count = 0 then
            topAsc.Insert (i, v)
            if topAsc.Count > topN then
                topAsc.RemoveAt 0

    for line in input do
        if line = "" then
            i <- i + 1
            updateTop calories
            calories <- 0
        else
            calories <- calories + int line
    
    if calories <> 0 then
        updateTop calories
    
    let mutable total = 0
    for v in topAsc do
        total <- total + v
    total

let input =
    "sample.txt"
    |> File.ReadLines

let part1Timed title =
    timeit (fun ms calories ->
        printfn "Part 1 - %s - %.3f ms: Max %i" title ms calories)

let part2Timed title =
    timeit (fun ms calories ->
        printfn "Part 2 - %s - %.3f ms: Top 3 %i" title ms calories)

part1Timed "Functional" (fun () -> solveFunctional 1 input)
part1Timed "Imperative" (fun () -> solveImperative 1 input)

part2Timed "Functional" (fun () -> solveFunctional 3 input)
part2Timed "Imperative" (fun () -> solveImperative 3 input)
