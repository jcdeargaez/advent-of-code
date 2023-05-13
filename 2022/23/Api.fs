open System.Diagnostics
open System.IO

open Domain

let timeit f =
    let sw = Stopwatch.StartNew ()
    let r = f ()
    sw.Stop ()
    printfn "%f ms" sw.Elapsed.TotalMilliseconds
    r

let simulate stop elves =
    let rec round i directions elvesSoFar =
        let result = Round.run elvesSoFar directions
        if stop i result then i, result
        else round (i + 1) (Direction.cycle directions) result.Elves
    
    elves
    |> round 1 Direction.initial

let computeEmptyTilesAfterRounds rounds elves =
    let finalRound =
        elves
        |> simulate (fun i _ -> i >= rounds)
        |> snd
    
    finalRound.Elves
    |> Elves.computeEmptyTiles

let findEmptyMovesRound elves =
    elves
    |> simulate (fun _ result -> result.Moves = 0)
    |> fst

[<EntryPoint>]
let main _ =
    let elves = timeit (fun () ->
        "input.txt"
        |> File.ReadLines
        |> Elves.parse)

    printfn "Parsed elves"

    let part1 = timeit (fun () ->
        elves
        |> computeEmptyTilesAfterRounds 10)

    printfn "Part 1 - Empty tiles after 10 rounds: %i" part1

    let part2 = timeit (fun () ->
        elves
        |> findEmptyMovesRound)

    printfn "Part 2 - Rounds until no moves: %i" part2
    0
