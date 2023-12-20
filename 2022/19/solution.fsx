#r "nuget: FParsec"

open System.IO
open System.Collections.Generic

type Resource =
    | Ore = 0
    | Clay = 1
    | Obsidian = 2
    | Geode = 3

type RobotCost = int array

[<Struct>]
type Blueprint = {
    Id : int
    RobotCosts : RobotCost array
}

[<Struct>]
type SimulationState = {
    Minute : int
    Robots : int array
    Resources : int array
}

module Parser =
    open FParsec

    let [<Literal>] Ore = "ore"
    let [<Literal>] Clay = "clay"
    let [<Literal>] Obsidian = "obsidian"
    let [<Literal>] Geode = "geode"

    let parse input =
        let pcost = pint32 .>> skipChar ' ' .>>. choice [
            stringReturn Ore Resource.Ore
            stringReturn Clay Resource.Clay
            stringReturn Obsidian Resource.Obsidian
        ]
        let pmultiCost =
            sepBy1 pcost (skipString " and ")
            |>> fun costResources ->
                let costs = Array.replicate (int Resource.Geode + 1) 0
                costResources
                |> List.iter (fun (cost, resource) ->
                    let i = int resource
                    costs.[i] <- cost)
                costs
        
        let probot resource = skipString " Each " >>. skipString resource >>. skipString " robot costs " >>. pmultiCost .>> skipChar '.'
        let pblueprint =
            skipString "Blueprint " >>. pint32 .>> skipString ":"
            .>>. tuple4 (probot Ore) (probot Clay) (probot Obsidian) (probot Geode)
            |>> fun (tid, (oreRobotCost, clayRobotCost, obsidianRobotCost, geodeRobotCost)) ->
                { Id = tid; RobotCosts = [| oreRobotCost; clayRobotCost; obsidianRobotCost; geodeRobotCost |] }
        
        let pblueprintLine = pblueprint .>> skipNewline
        let pblueprintLineEof = many1 pblueprintLine .>> eof
        match input |> run pblueprintLineEof with
        | Success (b, _, _) -> b
        | Failure (errorMsg, _, _) -> failwith errorMsg

let simulate minutes blueprint =
    let maxOre = Array.max [|
        blueprint.RobotCosts.[int Resource.Ore].[int Resource.Ore]
        blueprint.RobotCosts.[int Resource.Clay].[int Resource.Ore]
        blueprint.RobotCosts.[int Resource.Obsidian].[int Resource.Ore]
        blueprint.RobotCosts.[int Resource.Geode].[int Resource.Ore]
    |]

    let createKey state =
        {
            state with
                Resources = [|
                    state.Resources.[int Resource.Ore] % (maxOre + 1)
                    state.Resources.[int Resource.Clay] % (blueprint.RobotCosts.[int Resource.Obsidian].[int Resource.Clay] + 1)
                    state.Resources.[int Resource.Obsidian] % (blueprint.RobotCosts.[int Resource.Geode].[int Resource.Obsidian] + 1)
                    state.Resources.[int Resource.Geode]
                |]
        }

    let tryBuyRobot state robotIdx =
        let robotCosts = blueprint.RobotCosts.[robotIdx]
        let hasDependantRobots =
            robotCosts
            |> Seq.indexed
            |> Seq.forall (fun (i, cost) -> cost = 0 || state.Robots.[i] > 0)
        if hasDependantRobots then
            let minutesToCost =
                robotCosts
                |> Seq.indexed
                |> Seq.filter (snd >> (<) 0)
                |> Seq.map (fun (i, cost) -> float (cost - state.Resources.[i]) / float state.Robots.[i] |> ceil |> int)
                |> Seq.max
            let minutesToRobot = 1 + max 0 minutesToCost
            let jumpToMinute = state.Minute + minutesToRobot
            if jumpToMinute < minutes then
                let newRobots = Array.copy state.Robots
                newRobots.[robotIdx] <- newRobots.[robotIdx] + 1
                let newResources =
                    state.Resources
                    |> Array.mapi (fun i resource -> resource + state.Robots.[i] * minutesToRobot - robotCosts.[i])
                Some { Minute = jumpToMinute; Resources = newResources; Robots = newRobots }
            else None
        else None

    let visited = HashSet () // More performant than F# Set
    let rec tick maxGeodesSoFar =
        function
        | [] -> maxGeodesSoFar
        | state :: states ->
            let key = createKey state
            let nextStates =
                if state.Minute >= minutes || visited.Contains key then List.empty
                else
                    let minutesRemaining = minutes - state.Minute
                    let collectingGeodesOnly =
                        if state.Robots.[int Resource.Geode] = 0 then None
                        else
                            Some {
                                state with
                                    Minute = state.Minute + minutesRemaining
                                    Resources =
                                        state.Resources
                                        |> Array.mapi (fun i resource -> resource + state.Robots.[i] * minutesRemaining)
                            }

                    let nextStatesWithRobots =
                        {0..int Resource.Geode}
                        |> Seq.map (tryBuyRobot state)
                        |> Seq.choose id
                        |> Seq.toList

                    let newStates =
                        nextStatesWithRobots
                        |> List.append (
                            collectingGeodesOnly
                            |> Option.map List.singleton
                            |> Option.defaultValue List.empty)
                        |> List.filter (fun s ->
                            s.Robots.[int Resource.Ore] <= maxOre &&
                            s.Robots.[int Resource.Clay] <= blueprint.RobotCosts.[int Resource.Obsidian].[int Resource.Clay] &&
                            s.Robots.[int Resource.Obsidian] <= blueprint.RobotCosts.[int Resource.Geode].[int Resource.Obsidian])
                        |> List.filter (fun s ->
                            let optimisticGeodesProjection = s.Resources.[int Resource.Geode] + s.Robots.[int Resource.Geode] * minutesRemaining + (minutesRemaining * (minutesRemaining - 1)) / 2
                            optimisticGeodesProjection >= maxGeodesSoFar)
                        |> List.filter (fun s -> createKey s |> visited.Contains |> not)
                    newStates
            
            visited.Add key |> ignore
            tick (max maxGeodesSoFar state.Resources.[int Resource.Geode]) (nextStates @ states)

    [{ Minute = 0; Robots = [| 1; 0; 0; 0 |]; Resources = [| 0; 0; 0; 0 |] }]
    |> tick 0

let qualityLevelAsync blueprint =
    async {
        let geodes =
            blueprint
            |> simulate 24
        return geodes * blueprint.Id
    }

let simulateAsync minutes blueprint =
    async {
        let geodes =
            blueprint
            |> simulate minutes
        return geodes
    }

let blueprints =
    File.ReadAllText "input.txt"
    |> Parser.parse

let part1 =
    blueprints
    |> Seq.map qualityLevelAsync
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.sum

printfn "Part 1: %i" part1

let part2 =
    blueprints
    |> Seq.truncate 3
    |> Seq.map (simulateAsync 32)
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.reduce ( * )

printfn "Part 2: %i" part2
