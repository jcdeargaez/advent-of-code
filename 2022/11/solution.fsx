open System.IO

type Monkey = {
    Index : int
    Operation : uint64 -> uint64
    TestDivisor : uint64
    TargetOnPositive : int
    TargetOnNegative : int
}

type Inspection = {
    Items : uint64 list
    Times : uint64
}

let parseOperation (line : string) =
    let parts = line.Split ' '
    let op =
        match parts.[1] with
        | "+" -> (+)
        | "*" -> ( * )
        | _ -> failwith "Unknown operator"
    fun old ->
        let sndArg = if parts.[2] = "old" then old else uint64 parts.[2]
        op old sndArg

let parseMonkey (lines : string array) =
    let index = int lines.[0].[7..lines.[0].Length - 2]
    let items =
        lines.[1].[18..].Split ','
        |> Seq.map uint64
        |> List.ofSeq
    let operation = parseOperation lines.[2].[19..]
    let div = lines.[3].Substring 21 |> uint64
    let onPositive = int lines.[4].[29..]
    let onNegative = int lines.[5].[30..]
    let monkey = {
        Index = index; Operation = operation; TestDivisor = div
        TargetOnPositive = onPositive; TargetOnNegative = onNegative }
    let initialInspection = { Items = items; Times = 0uL }
    monkey, initialInspection

let turn worryLevelReductor inspections monkey =
    let nextInspections = Array.copy inspections

    let inspect item =
        let worryLevel = monkey.Operation item |> worryLevelReductor
        let targetMonkey = if worryLevel % monkey.TestDivisor = 0uL then monkey.TargetOnPositive else monkey.TargetOnNegative
        let inspection = nextInspections.[targetMonkey]
        nextInspections.[targetMonkey] <- { nextInspections.[targetMonkey] with Items = worryLevel :: inspection.Items }

    inspections.[monkey.Index].Items
    |> List.iter inspect

    let inspection = nextInspections.[monkey.Index]
    nextInspections.[monkey.Index] <- { inspection with Items = List.empty; Times = inspection.Times + uint64 inspection.Items.Length }
    nextInspections

let round worryLevelReductor monkeys inspections =
    monkeys
    |> Array.fold (turn worryLevelReductor) inspections

let infiniteRounds worryLevelReductor monkeys initialInspections =
    let rec loop inspectionsSoFar =
        seq {
            let roundInspections = round worryLevelReductor monkeys inspectionsSoFar
            yield roundInspections
            yield! loop roundInspections
        }
    loop initialInspections

let run worryLevelReductor rounds monkeys inspections =
    let roundInspections =
        infiniteRounds worryLevelReductor monkeys inspections
        |> Seq.take rounds
        |> Seq.last

    let monkeyBusiness =
        roundInspections
        |> Seq.map (fun inspection -> inspection.Times)
        |> Seq.sortDescending
        |> Seq.take 2
        |> Seq.reduce ( * )

    monkeyBusiness

let part1 =
    let worryLevelReductor wl = wl / 3uL
    run worryLevelReductor 20

let part2 monkeys =
    let commonDivisor =
        monkeys
        |> Seq.map (fun monkey -> monkey.TestDivisor)
        |> Seq.reduce ( * )
    
    let worryLevelReductor wl = wl % commonDivisor
    run worryLevelReductor 10_000 monkeys

let monkeys, initialInspections =
    File.ReadLines "input.txt"
    |> Seq.chunkBySize 7
    |> Seq.map parseMonkey
    |> Seq.toArray
    |> Array.unzip

part1 monkeys initialInspections
|> printfn "Part 1: %i"

part2 monkeys initialInspections
|> printfn "Part 2: %i"
