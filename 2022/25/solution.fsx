open System
open System.IO

module Snafu =
    let private scale = [| '0'; '1'; '2'; '='; '-'; '0'; '1'; '2'; '='; '-'; '0' |]
    let [<Literal>] private ZeroIdx = 5

    let parseSnafuDigit = function
        | '=' -> -2
        | '-' -> -1
        | '0' -> 0
        | '1' -> 1
        | '2' -> 2
        | c -> failwith $"Invalid character '{c}'"

    let add a b =
        let result = Array.zeroCreate (1 + max (String.length a) (String.length b))
        let rec loop i j carry =
            if i < 0 && j < 0 then carry
            else
                let da = if i >= 0 then parseSnafuDigit a.[i] else 0
                let db = if j >= 0 then parseSnafuDigit b.[j] else 0
                let p = carry + da + db + ZeroIdx
                let carry' = if p < 3 then -1 elif p > 7 then 1 else 0
                result.[1 + max i j] <- scale.[p]
                loop (i - 1) (j - 1) carry'

        let finalCarry = loop (a.Length - 1) (b.Length - 1) 0
        result.[0] <- scale.[finalCarry + ZeroIdx]
        let trimmed = (String result).TrimStart '0'
        if trimmed.Length > 0 then trimmed else "0"

    let toB10 (a : string) =
        let rec loop i f acc =
            if i < 0 then acc
            else
                let digit = parseSnafuDigit a.[i]
                loop (i - 1) (f * 5) (acc + digit * f)

        loop (a.Length - 1) 1 0

module Operations =
    let part1 input =
        input
        |> Seq.reduce Snafu.add

File.ReadLines("input.txt")
|> Operations.part1
|> printfn "Part 1: %s"
