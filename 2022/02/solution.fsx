open System.Diagnostics
open System.IO

module Naive =
    type Shape =
        | Rock
        | Paper
        | Scissors

    type Outcome =
        | Lose
        | Draw
        | Win

    let private outcome shape1 shape2 =
        match shape1, shape2 with
        | Rock    , Scissors
        | Paper   , Rock
        | Scissors, Paper        -> Win
        | _ when shape1 = shape2 -> Draw
        | _                      -> Lose

    let parseShape = function
        | 'A' | 'X' -> Rock
        | 'B' | 'Y' -> Paper
        | 'C' | 'Z' -> Scissors
        | _ -> failwith "Unknown shape"

    let parseOutcome = function
        | 'X' -> Lose
        | 'Y' -> Draw
        | 'Z' -> Win
        | _ -> failwith "Unknown outcome"

    let determineShape opponentShape outcome =
        match opponentShape, outcome with
        | Rock    , Win  -> Paper
        | Paper   , Win  -> Scissors
        | Scissors, Win  -> Rock
        | Rock    , Lose -> Scissors
        | Paper   , Lose -> Rock
        | Scissors, Lose -> Paper
        | _, Draw        -> opponentShape

    let round shape1 shape2 =
        let shapePoints =
            match shape1 with
            | Rock     -> 1
            | Paper    -> 2
            | Scissors -> 3
        
        let outcomePoints =
            match outcome shape1 shape2 with
            | Lose -> 0
            | Draw -> 3
            | Win  -> 6

        shapePoints + outcomePoints

module Efficient =
    [<Measure>] type private shape
    [<Measure>] type private outcome

    let [<Literal>] Rock = 1<shape>    
    let [<Literal>] Paper = 2<shape>
    let [<Literal>] Scissors = 3<shape>

    let [<Literal>] Win = 6<outcome>
    let [<Literal>] Draw = 3<outcome>
    let [<Literal>] Lose = 0<outcome>

    let inline private outcome shape1 shape2 =
        match shape1, shape2 with
        | Rock    , Scissors
        | Paper   , Rock
        | Scissors, Paper                -> Win
        | _ when int shape1 = int shape2 -> Draw
        | _                              -> Lose

    let parseShape letter =
        let c = if letter <= 'C' then '@' else 'W' // 'A' and 'X' - 1, respectively
        (int letter - int c) * 1<shape>

    let parseOutcome letter =
        (int letter - int 'X') * 3<outcome>

    let determineShape opponentShape outcome =
        match opponentShape, outcome with
        | Rock    , Win  -> Paper
        | Paper   , Win  -> Scissors
        | Scissors, Win  -> Rock
        | Rock    , Lose -> Scissors
        | Paper   , Lose -> Rock
        | Scissors, Lose -> Paper
        | _, Draw        -> opponentShape
        | _ -> failwith "Unknown shape or outcome"

    let round (shape1 : int<shape>) (shape2 : int<shape>) =
        int shape1 + int (outcome shape1 shape2)

module Api =
    let read (lines : string seq) =
        lines
        |> Seq.map (fun line -> line.[0], line.[2])

    let simulate (round : char * char -> int) input =
        input
        |> Seq.sumBy round

let naiveAdapterPart1 (opponentShape, myShape) =
    let shape1 = Naive.parseShape myShape
    let shape2 = Naive.parseShape opponentShape
    Naive.round shape1 shape2

let efficientAdapterPart1 (opponentShape, myShape) =
    let shape1 = Efficient.parseShape myShape
    let shape2 = Efficient.parseShape opponentShape
    Efficient.round shape1 shape2

let naiveAdapterPart2 (opponentShape, outcome) =
    let shape2 = Naive.parseShape opponentShape
    let shape1 = Naive.determineShape shape2 (Naive.parseOutcome outcome)
    Naive.round shape1 shape2

let efficientAdapterPart2 (opponentShape, outcome) =
    let shape2 = Efficient.parseShape opponentShape
    let shape1 = Efficient.determineShape shape2 (Efficient.parseOutcome outcome)
    Efficient.round shape1 shape2

let timeit printer f =
    let sw = Stopwatch.StartNew ()
    let r = f ()
    let duration = sw.Elapsed.TotalMilliseconds
    printer r duration

let input =
    "input.txt"
    |> File.ReadLines
    |> Api.read

let printer title =
    printfn "  %s: %i - %.5f ms" title

printfn "Part 1"
{0..2} |> Seq.iter (fun i ->
    printfn " Running %s" (if i = 0 then "cold start" else string i)
    timeit (printer "Naive") (fun () -> Api.simulate naiveAdapterPart1 input)
    timeit (printer "Efficient") (fun () -> Api.simulate efficientAdapterPart1 input)
)

printfn "\nPart 2"
{0..2} |> Seq.iter (fun i ->
    printfn " Running %s" (if i = 0 then "cold start" else string i)
    timeit (printer "Naive") (fun () -> Api.simulate naiveAdapterPart2 input)
    timeit (printer "Efficient") (fun () -> Api.simulate efficientAdapterPart2 input)
)
