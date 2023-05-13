module Round

open System.Collections.Generic

open Domain

let proposeAction (elves : Elves) tile directions =
    let inline areElvesAround (elves : Elves) =
        Array.exists elves.Contains

    let rec next i =
        if i = Array.length directions then Stay tile
        else
            let direction = directions.[i]
            if tile |> Elves.frontTiles direction |> areElvesAround elves then next (i + 1)
            else
                let frontTile = direction.Front tile
                Move {Source = tile; Target = frontTile}
    
    if tile |> Elves.neighbourTiles |> areElvesAround elves then next 0
    else Stay tile

let run (elves : Elves) directions =
    let targets = Dictionary ()
    let newElves = HashSet ()

    let processElf moveProposalsSoFar tile =
        match proposeAction elves tile directions with
        | Stay t ->
            newElves.Add t |> ignore
            moveProposalsSoFar
        | Move mp ->
            match targets.TryGetValue mp.Target with
            | true, times -> targets.[mp.Target] <- times + 1
            | _           -> targets.[mp.Target] <- 1
            mp :: moveProposalsSoFar

    let processMoveProposal mp =
        match targets.TryGetValue mp.Target with
        | true, 1 -> newElves.Add mp.Target |> ignore; 1
        | _       -> newElves.Add mp.Source |> ignore; 0

    let moves =
        elves
        |> Seq.fold processElf List.empty
        |> List.sumBy processMoveProposal
    
    {Elves = newElves; Moves = moves}
