open System
open System.IO
open System.Collections.Generic

let moveMultipleCrates n src tgt (stacks : Stack<char> array) =
    Array.init n (fun _ -> stacks.[src].Pop ())
    |> Seq.rev
    |> Seq.iter (stacks.[tgt].Push)

let rec moveSingleCrate times src tgt (stacks : Stack<char> array) =
    if times > 0 then
        stacks.[src].Pop () |> stacks.[tgt].Push
        moveSingleCrate (times - 1) src tgt stacks

let updateStacks (stacks : Stack<char> array) line =
    1
    |> Seq.unfold (fun i -> if i < String.length line then Some (line.[i], i + 4) else None)
    |> Seq.iteri (fun i c -> if c <> ' ' then stacks.[i].Push c)

let parseMove move (line : string) (stacks : Stack<char> array) =
    let parts = line.Split ' '
    stacks
    |> move (int parts.[1]) (int parts.[3] - 1) (int parts.[5] - 1)

let initStacks lines =
    let stacks = Array.init ((String.length (lines |> Array.item 0) + 1) / 4) (fun _ -> Stack<char> ())
    lines |> Array.iter (updateStacks stacks)
    stacks

let processMoves stacks1 stacks2 lines =
    lines
    |> Seq.iter (fun line ->
        parseMove moveSingleCrate line stacks1
        parseMove moveMultipleCrates line stacks2)

let processInput crates moves =
    let stacks1 = initStacks crates
    let stacks2 = initStacks crates
    processMoves stacks1 stacks2 moves
    stacks1, stacks2

let answer (stacks : Stack<char> array) =
    stacks
    |> Array.map (fun stack -> stack.Pop ())
    |> String

let lines = File.ReadAllLines "input.txt"
let crates, moves =
    lines
    |> Array.splitAt (lines |> Array.findIndex (String.length >> (=) 0))

let crates' =
    crates
    |> Array.rev
    |> Array.tail

let moves' =
    moves
    |> Array.tail

let part1, part2 = processInput crates' moves'
printfn "Part 1: %s" (answer part1)
printfn "Part 2: %s" (answer part2)
