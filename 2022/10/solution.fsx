open System
open System.IO

type Instruction =
    | Noop
    | AddX of int

type Cpu = {
    Cycle : int
    X : int
}

module Cpu =
    let initial = {Cycle = 1; X = 1}

    let execute cpu = function
        | Noop ->   [| {cpu with Cycle = cpu.Cycle + 1} |]
        | AddX v -> [| {cpu with Cycle = cpu.Cycle + 1}; {Cycle = cpu.Cycle + 2; X = cpu.X + v} |]
    
    let signalStrength cpu = cpu.Cycle * cpu.X

module Instruction =
    let parse line =
        match line with
        | "noop" -> Noop
        | _ ->
            match line.Split ' ' with
            | [| "addx"; v |] -> int v |> AddX
            | _ -> failwith $"Unknown instruction: '{line}'"

module Crt =
    let initial = Array.init 6 (fun _ -> Array.replicate 40 '.')

    let toString (crt : char array array) =
        crt
        |> Seq.map String
        |> String.concat "\n"

let input =
    File.ReadLines "input.txt"
    |> Seq.map Instruction.parse

let part1, part2 =
    let bindExecute cpus = Cpu.execute (Array.last cpus)

    let parts (signalStrenghSoFar, (screenSoFar : char array array)) cpu =
        let signalStrength = if (cpu.Cycle - 20) % 40 = 0 then Cpu.signalStrength cpu else 0
        if cpu.Cycle <= 240 then
            let i, j = Math.DivRem (cpu.Cycle - 1, 40)
            if cpu.X - 1 <= j && j <= cpu.X + 1 then
                screenSoFar.[i].[j] <- '#'
        signalStrenghSoFar + signalStrength, screenSoFar

    input
    |> Seq.scan bindExecute [| Cpu.initial |]
    |> Seq.collect id
    |> Seq.fold parts (0, Crt.initial)

printfn "Part 1: %i" part1
printfn "Part 2:"
printfn "%s" (Crt.toString part2)
