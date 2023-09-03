open System.Collections.Generic
open System.IO

type Position = {
    X : int
    Y : int
}

type Direction = Up | Down | Left | Right

type Motion = {
    Direction : Direction
    Steps : int
}

module Direction =
    let parse = function
        | "U" -> Up
        | "D" -> Down
        | "L" -> Left
        | "R" -> Right
        |  x -> failwith $"Unknown direction '{x}'"

module Motion =
    let parse (str : string) =
        match str.Split ' ' with
        | [| dir; steps |] -> {Direction = Direction.parse dir; Steps = int steps}
        | _ -> failwith $"Invalid format '{str}'"

    let moveHead dir head =
        let dx, dy =
            match dir with
            | Up    ->  0,  1
            | Down  ->  0, -1
            | Left  -> -1,  0
            | Right ->  1,  0
        {X = head.X + dx; Y = head.Y + dy}
    
    let moveTail head tail =
        let dx = head.X - tail.X
        let dy = head.Y - tail.Y
        let dx', dy' =
            match abs dx, abs dy with
            // Tail needs to move
            | 2  , 2                 -> dx / 2, dy / 2
            | 2  , ady when ady <= 1 -> dx / 2, dy
            | adx, 2   when adx <= 1 -> dx    , dy / 2
            // Tail does not need to move
            | adx, ady when adx + ady <= 2 -> 0, 0
            | _ -> failwith (sprintf "Tail (%i, %i) too far from head (%i, %i)" tail.X tail.Y head.X head.Y)
        {X = tail.X + dx'; Y = tail.Y + dy'}

    let move (visited : HashSet<Position>) (knots : Position array) motion =
        let rec iterate step knotIdx =
            if step > 0 then
                let step', knotIdx' =
                    if knotIdx = 0 then
                        knots.[0] <- moveHead motion.Direction knots.[0]
                        step, knotIdx + 1
                    else
                        let tail = knots.[knotIdx]
                        let tail' = moveTail knots.[knotIdx - 1] tail
                        if tail = tail' then step - 1, 0
                        else
                            knots.[knotIdx] <- tail'
                            if knotIdx < knots.Length - 1 then step, knotIdx + 1
                            else
                                visited.Add tail' |> ignore
                                step - 1, 0
                iterate step' knotIdx'
        iterate motion.Steps 0

let origin = {X = 0; Y = 0}
let visited1 = HashSet ()
visited1.Add origin |> ignore
let visited2 = HashSet ()
visited2.Add origin |> ignore

let movePart1 = Motion.move visited1 (Array.replicate 2 origin)
let movePart2 = Motion.move visited2 (Array.replicate 10 origin)

let processLine line =
    let motion = Motion.parse line
    movePart1 motion
    movePart2 motion

File.ReadAllLines "input.txt"
|> Seq.iter processLine

printfn "Part 1: %i" visited1.Count
printfn "Part 2: %i" visited2.Count
