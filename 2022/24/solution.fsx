open System.Collections.Generic
open System.IO

[<AutoOpen>]
module Domain =
    type Direction = Up | Down | Left | Right
    type Valley = { Width: int; Height: int }
    type Position = { X: int; Y: int; Direction: Direction }
    type Blizzards = { Positions : Position HashSet }
    type BlizzardsStates = { States : Blizzards array }
    type Point = { X: int; Y: int }
    type Move = { Point : Point; Step : int }

module Direction =
    let parse = function
        | '^' -> Some Up
        | 'v' -> Some Down
        | '<' -> Some Left
        | '>' -> Some Right
        | _ -> None

module Position =
    let create valley x y dir =
        if 0 < x && x < valley.Width - 1 && 0 < y && y < valley.Height - 1
        then { X = x; Y = y; Direction = dir }
        else failwith $"Invalid coordinates ({x}, {y}) on valley ({valley.Width}, {valley.Height})"
    
    let move valley blizzard =
        match blizzard.Direction with
        | Up -> if blizzard.Y > 1 then { blizzard with Y = blizzard.Y - 1 } else { blizzard with Y = valley.Height - 2 }
        | Down -> if blizzard.Y < valley.Height - 2 then { blizzard with Y = blizzard.Y + 1 } else { blizzard with Y = 1 }
        | Left -> if blizzard.X > 1 then { blizzard with X = blizzard.X - 1 } else { blizzard with X = valley.Width - 2 }
        | Right -> if blizzard.X < valley.Width - 2 then { blizzard with X = blizzard.X + 1 } else { blizzard with X = 1 }

module Parser =
    let parseLine valley y line =
        line
        |> Seq.mapi (fun x -> Direction.parse >> Option.map (Position.create valley x y))
        |> Seq.choose id

    let parse (lines : string array) =
        let width = lines.[0].Length
        let height = lines.Length
        let valley = { Width = width; Height = height }
        let positions =
            lines
            |> Seq.mapi (parseLine valley)
            |> Seq.concat
            |> HashSet
        let blizzards = { Positions = positions }
        valley, blizzards

module Valley =
    let tick valley blizzards =
        let positions =
            blizzards.Positions
            |> Seq.map (Position.move valley)
            |> HashSet
        { Positions = positions }

module BlizzardState =
    let computeStates valley initialBlizzards =
        let lcm a b =
            let rec gcd a b =
                if b = 0 then a else gcd b (a % b)
            (a * b) / (gcd a b)
        let minutes = lcm (valley.Width - 2) (valley.Height - 2)
        let states =
            {1..minutes}
            |> Seq.scan (fun blizzardsSoFar _ -> Valley.tick valley blizzardsSoFar) initialBlizzards
            |> Seq.tail
            |> Seq.toArray
        let blizzardStates = { States = states }
        blizzardStates

    let contains point blizzardState =
        blizzardState.Positions.Contains { X = point.X; Y = point.Y; Direction = Up }
        || blizzardState.Positions.Contains { X = point.X; Y = point.Y; Direction = Down }
        || blizzardState.Positions.Contains { X = point.X; Y = point.Y; Direction = Left }
        || blizzardState.Positions.Contains { X = point.X; Y = point.Y; Direction = Right }

module Operations =
    let traverse valley blizzardStates origin target initialStep =
        let visited = HashSet()
        let pending = Queue()
        let dirs = [| Up; Down; Left; Right |]
        let rec loop () =
            let move = pending.Dequeue()
            let normalizedMove = { move with Step = move.Step % blizzardStates.States.Length }
            if visited.Contains normalizedMove then loop ()
            elif move.Point = target then move
            else
                visited.Add normalizedMove |> ignore
                let nextStep = move.Step + 1
                let nextBlizzardState = blizzardStates.States.[nextStep % blizzardStates.States.Length]
                let p = move.Point
                dirs
                |> Seq.choose (function
                    | Up when p.Y > 1 || (p.Y = 1 && p.X = 1) -> Some { X = p.X; Y = p.Y - 1 }
                    | Down when p.Y < valley.Height - 2 || (p.Y = valley.Height - 2 && p.X = valley.Width - 2) -> Some { X = p.X; Y = p.Y + 1 }
                    | Left when p.X > 1 && 0 < p.Y && p.Y < valley.Height - 1 -> Some { X = p.X - 1; Y = p.Y }
                    | Right when p.X < valley.Width - 2 && 0 < p.Y && p.Y < valley.Height - 1 -> Some { X = p.X + 1; Y = p.Y }
                    | _ -> None)
                |> Seq.filter (fun p -> nextBlizzardState |> BlizzardState.contains p |> not)
                |> Seq.iter (fun p -> pending.Enqueue { Point = p; Step = nextStep })
                
                if nextBlizzardState |> BlizzardState.contains move.Point |> not then
                    pending.Enqueue { Point = move.Point; Step = nextStep }
                
                loop ()
        
        pending.Enqueue { Point = origin; Step = initialStep }
        loop ()

    let part1 valley blizzardStates =
        let origin = { X = 1; Y = 0 }
        let target = { X = valley.Width - 2; Y = valley.Height - 1 }
        let move = traverse valley blizzardStates origin target -1
        move.Step + 1

    let part2 valley blizzardStates =
        let origin = { X = 1; Y = 0 }
        let target = { X = valley.Width - 2; Y = valley.Height - 1 }
        [
            origin, target
            target, origin
            origin, target
        ]
        |> Seq.fold (fun stepSoFar (origin, target) ->
            let move = traverse valley blizzardStates origin target stepSoFar
            move.Step) -1
        |> (+) 1

let valley, initialBlizzards =
    File.ReadAllLines "input.txt"
    |> Parser.parse

let blizzardStates = BlizzardState.computeStates valley initialBlizzards

Operations.part1 valley blizzardStates
|> printfn "Part 1: %i"

Operations.part2 valley blizzardStates
|> printfn "Part 2: %i"
