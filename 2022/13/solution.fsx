open System.IO
open System.Text

type Packet =
    | Number of int
    | Numbers of Packet list

let dividers = [
    Numbers [Numbers [Number 2]]
    Numbers [Numbers [Number 6]]
]

let parsePacket (line : string) =
    let sb = StringBuilder ()

    let rec parseList i packetSoFar =
        let c = line.[i]
        if c = ']' then
            let packetSoFar' =
                if sb.Length = 0 then packetSoFar
                else (sb.ToString () |> int |> Number) :: packetSoFar
            i, packetSoFar' |> List.rev |> Numbers
        else
            let i', packetSoFar' =
                match c with
                | '[' ->
                    let ni, np = parseList (i + 1) []
                    ni, np :: packetSoFar
                | ',' ->
                    if sb.Length = 0 then i, packetSoFar
                    else
                        let n = sb.ToString () |> int |> Number
                        sb.Clear () |> ignore
                        i, n :: packetSoFar
                | digit ->
                    sb.Append digit |> ignore
                    i, packetSoFar
            
            parseList (i' + 1) packetSoFar'

    parseList 1 []
    |> snd

let rec compare (left, right) =
    match left, right with
    | Number a, Number b   -> if a < b then -1 elif a > b then 1 else 0
    | Number _, Numbers _  -> compare (Numbers [left], right)
    | Numbers _, Number _  -> compare (left, Numbers [right])
    | Numbers a, Numbers b ->
        Seq.zip a b
        |> Seq.map compare
        |> Seq.tryFind ((<>) 0)
        |> Option.defaultWith (fun () -> compare (Number a.Length, Number b.Length))

let part1 packets =
    packets
    |> Seq.chunkBySize 2
    |> Seq.mapi (fun i a -> if compare (a.[0], a.[1]) = -1 then i + 1 else 0)
    |> Seq.sum

let part2 packets =
    let lessers = Array.replicate dividers.Length 1
    let updateLessers packet =
        dividers
        |> List.iteri (fun i p ->
            if compare (packet, p) = -1 then
                lessers.[i] <- lessers.[i] + 1)

    dividers @ packets
    |> List.iter updateLessers

    lessers
    |> Array.reduce ( * )

let readInput lines =
    lines
    |> Seq.filter (String.length >> (<>) 0)
    |> Seq.map parsePacket

{0..2}
|> Seq.iter (fun i ->
    printfn "Running %s" (if i = 0 then "cold start" else string i)
    let sw = System.Diagnostics.Stopwatch.StartNew ()

    let packets =
        File.ReadLines "input.txt"
        |> readInput
        |> Seq.toList 
    let t1 = sw.Elapsed.TotalMilliseconds

    let p1 = part1 packets
    let t2 = sw.Elapsed.TotalMilliseconds

    let p2 = part2 packets
    let t3 = sw.Elapsed.TotalMilliseconds 

    printfn "Read input %.5f ms" t1
    printfn "Part 1 %i %.5f ms" p1 (t2 - t1)
    printfn "Part 2 %i %.5f ms" p2 (t3 - t2)
    printfn ""
)
