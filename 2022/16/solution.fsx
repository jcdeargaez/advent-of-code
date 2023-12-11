#r "nuget:FParsec"

open System.IO

type Valve = {
    Name : string
    Rate : int
    LeadsTo : string Set
}

type SimulationState = {
    OpenedValves : int Set
    CurrentValveIdx : int
    Pressure : int
    MinutesRemaining : int
}

module Parser =
    open FParsec

    let parse input =
        let pvalve : Parser<string, unit> = anyString 2
        let ptargetValves =
            let psingleTargetValve = skipString " leads to valve " >>. pvalve |>> List.singleton
            let pmanyTargetValves = skipString "s lead to valves " >>. sepBy1 pvalve (skipString ", ")
            skipString "; tunnel" >>. (psingleTargetValve <|> pmanyTargetValves)
        let pflowRate = skipString " has flow rate=" >>. pint32
        let pscanOutput =
            skipString "Valve " >>. tuple3 pvalve pflowRate ptargetValves
            |>> fun (name, rate, leadsTo) -> { Name = name; Rate = rate; LeadsTo = Set leadsTo }
        let pscanOutputLine = pscanOutput .>> skipNewline
        let pscanOutputLinesEof = many pscanOutputLine .>> eof

        match input |> run pscanOutputLinesEof with
        | Success (valves, _, _) -> valves
        | Failure (errorMsg, _, _) -> failwith errorMsg

let valves =
    File.ReadAllText "input.txt"
    |> Parser.parse
    |> Seq.map (fun v -> v.Name, v)
    |> Map.ofSeq

let indices, distances =
    let indices =
        valves
        |> Map.values
        |> Array.ofSeq

    let m = Array.init valves.Count (fun i ->
        Array.init valves.Count (fun j ->
            if i = j then 0.0
            else
                let a = indices.[i]
                let b = indices.[j]
                if a.LeadsTo |> Set.contains b.Name
                then 1.0
                else infinity))

    Seq.allPairs {0..valves.Count - 1} {0..valves.Count - 1}
    |> Seq.allPairs {0..valves.Count - 1}
    |> Seq.iter (fun (k, (i, j)) ->
        m.[i].[j] <- min m.[i].[j] (m.[i].[k] + m.[k].[j]))
    
    indices, m

let part1 totalMinutes usableValves =
    let rec next maxPressureSoFar =
        function
        | [] -> maxPressureSoFar
        | state :: remainingStates ->
            let nextStates =
                distances.[state.CurrentValveIdx]
                |> Seq.indexed
                |> Seq.filter (fun (vidx, d) ->
                    let valve = indices.[vidx]
                    let usable = usableValves |> Set.contains vidx
                    let alreadyOpened = state.OpenedValves |> Set.contains vidx
                    let reachableInTime = state.MinutesRemaining > int d
                    usable && state.CurrentValveIdx <> vidx && valve.Rate > 0 && not alreadyOpened && reachableInTime)
                |> Seq.map (fun (vidx, d) ->
                    let valve = indices.[vidx]
                    let minutes = state.MinutesRemaining - int d - 1
                    let pressure = state.Pressure + valve.Rate * minutes
                    let opened = state.OpenedValves |> Set.add vidx
                    { OpenedValves = opened; CurrentValveIdx = vidx; Pressure = pressure; MinutesRemaining = minutes })
                |> Seq.toList

            let maxPressure = max state.Pressure maxPressureSoFar
            next maxPressure (nextStates @ remainingStates)

    let initialSimulation = { OpenedValves = Set.empty; CurrentValveIdx = 0; Pressure = 0; MinutesRemaining = totalMinutes }
    List.singleton initialSimulation
    |> next 0

let part2 () =
    let relevantValves =
        valves
        |> Map.values
        |> Seq.indexed
        |> Seq.filter (fun (i, v) -> v.Rate > 0)
        |> Seq.map fst
        |> Set.ofSeq
    
    let rec combinations i k q =
        if i = k then q
        else
            let q' =
                Seq.allPairs relevantValves q
                |> Seq.map (fun (idx, combination) -> combination |> Set.add idx)
                |> Seq.fold (fun combinationsSoFar combination ->
                    combinationsSoFar
                    |> Set.add combination) Set.empty
            combinations (i + 1) k q'

    let initialCombination = Set.singleton 0 |> Set.singleton
    let subgraphs = combinations 0 (Set.count relevantValves / 2) initialCombination

    let maxPressure =
        subgraphs
        |> Set.fold (fun maxPressureSoFar subgraphA ->
            let subgraphB = subgraphA |> Set.difference relevantValves
            let maxPressureA = part1 26 subgraphA
            let maxPressureB = part1 26 subgraphB
            max (maxPressureA + maxPressureB) maxPressureSoFar) 0

    maxPressure

Set {0..indices.Length - 1}
|> part1 30
|> printfn "Part 1: %i"

part2 ()
|> printfn "Part 2: %i"
