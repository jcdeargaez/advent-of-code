open System
open System.IO
open System.Collections.Generic

[<Literal>]
let TotalSize = 70_000_000

[<Literal>]
let RequiredFreeSpace = 30_000_000

let readInput (lines : string seq) =
    let dirSize = Dictionary ()

    let rec updateDirectories (path : string option) size =
        match path with
        | Some p ->
            let curSize = p |> (dirSize.TryGetValue >> snd)
            dirSize.[p] <- curSize + size
            updateDirectories (Path.GetDirectoryName p |> Option.ofObj) size
        | _ -> ()

    let walk (pathSoFar : string) (line : string) =
        if line.StartsWith "$" then
            match line.[5..] with
            | ".." -> Path.GetDirectoryName pathSoFar
            | dir  -> Path.Combine (pathSoFar, dir)
        else
            let fileSize = line.[..line.IndexOf ' '] |> int
            updateDirectories (Some pathSoFar) fileSize
            pathSoFar

    lines
    |> Seq.filter (fun line -> line.StartsWith "$ cd" || (line.Length <> 0 && Char.IsDigit line.[0]))
    |> Seq.scan walk ""
    |> Seq.iter ignore

    dirSize

let part1 (dirs : Dictionary<string, int>) =
    dirs.Values
    |> Seq.filter ((>=) 100_000)
    |> Seq.sum

let part2 (dirs : Dictionary<string, int>) =
    let freeSpace = TotalSize - dirs.["/"]
    let spaceNeeded = RequiredFreeSpace - freeSpace
    dirs.Values
    |> Seq.filter ((<=) spaceNeeded)
    |> Seq.min

let dirs =
    File.ReadLines "input.txt"
    |> readInput

part1 dirs
|> printfn "Part 1: %i"

part2 dirs
|> printfn "Part 2: %i"
