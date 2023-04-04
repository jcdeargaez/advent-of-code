module Round

open Domain

let run grove directions =
    let processElf stateSoFar tile =
        let (actionsSoFar, targetsSoFar) = stateSoFar
        let action = Action.compute grove tile directions
        let newTargets =
            match action with
            | Stay _ -> targetsSoFar
            | Move mp ->
                targetsSoFar
                |> Map.change mp.Target (function
                    | Some times -> times + 1 |> Some
                    | None -> Some 0)
        action :: actionsSoFar, newTargets

    let (actions, targets) =
        grove.Elves
        |> Set.fold processElf (List.empty, Map.empty)
    
    let collisions =
        targets
        |> Map.filter (fun _ times -> times > 0)
        |> Map.keys
        |> Set

    let elves =
        actions
        |> List.map (function
            | Stay t -> t
            | Move mp -> if collisions.Contains mp.Target then mp.Source else mp.Target)
        |> Set
    
    let newGrove = {grove with Elves = elves}
    let minRectangle = Grove.computeMinimumRectangle newGrove
    {Directions = directions; Actions = actions; Collisions = collisions;
        Grove = newGrove; MinimumGroveRectangle = minRectangle}
