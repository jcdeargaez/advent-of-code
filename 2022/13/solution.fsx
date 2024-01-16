#r "nuget: FParsec"

open System.IO

type Packet =
    | Number of int
    | Numbers of Packet list

let dividers = [
    Numbers [Numbers [Number 2]]
    Numbers [Numbers [Number 6]]
]

module Parser =
    open FParsec

    let private ppacket =
        let pnumber = pint32 |>> Number
        let pnumbers, pnumbersref = createParserForwardedToRef ()
        let pitem = pnumber <|> pnumbers
        let items = sepBy pitem (skipChar ',')
        let pitems = between (skipChar '[') (skipChar ']') items |>> Numbers
        pnumbersref.Value <- pitems
        pnumbers
    
    let private ppacketEof = ppacket .>> eof

    let parsePacket line =
        match line |> run ppacketEof with
        | Success (packet, _, _) -> packet
        | Failure (errorMsg, _, _) -> failwith errorMsg

let rec compare left right =
    match left, right with
    | Number a, Number b   -> if a < b then -1 elif a > b then 1 else 0
    | Number _, Numbers _  -> compare (Numbers [left]) right
    | Numbers _, Number _  -> compare left (Numbers [right])
    | Numbers a, Numbers b ->
        Seq.zip a b
        |> Seq.map ((<||) compare)
        |> Seq.tryFind ((<>) 0)
        |> Option.defaultWith (fun () -> compare (Number a.Length) (Number b.Length))

let part1 packets =
    packets
    |> Seq.chunkBySize 2
    |> Seq.mapi (fun i a -> if compare a.[0] a.[1] = -1 then i + 1 else 0)
    |> Seq.sum

let part2 packets =
    let lessers = Array.replicate dividers.Length 1
    let updateLessers packet =
        dividers
        |> List.iteri (fun i p ->
            if compare packet p = -1 then
                lessers.[i] <- lessers.[i] + 1)

    dividers @ packets
    |> List.iter updateLessers

    lessers
    |> Array.reduce ( * )

let packets =
    File.ReadLines "input.txt"
    |> Seq.filter (String.length >> (<>) 0)
    |> Seq.map Parser.parsePacket
    |> Seq.toList

part1 packets
|> printfn "Part 1: %i"

part2 packets
|> printfn "Part 2: %i"
