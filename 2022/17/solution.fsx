open System
open System.Diagnostics
open System.Collections.Generic
open System.IO

type Gust = Left | Right

[<Measure>] type wall
[<Measure>] type rock
[<Measure>] type level
[<Measure>] type block

type Chamber = {
    JetIdx : int
    Levels : ResizeArray<byte<level>>
    Rocks : int64
    Height : int64
    Cache : Dictionary<CacheKey, CacheValue>
}

and CacheKey = {
    JetIdx : int
    Rock : int<rock>
    TopLevels : int64<block>
}

and CacheValue = {
    Height : int64
    RocksAdded : int64
}

module Unit =
    let inline rock a = a * 1<rock>
    let inline level a = a * 1uy<level>
    let inline block a = a * 1<block>
    let inline topBlock a = a * 1L<block>

module Jet =
    let parse gas =
        let jets =
            gas
            |> Seq.map (function
                | '>' -> Right
                | '<' -> Left
                | c -> failwith $"Unknown gust '{c}'")
            |> Seq.toArray
        
        let gust i =
            let nextIdx = (i + 1) % jets.Length
            nextIdx, jets.[i]

        gust

module Rock =

    let shapes =
        let rocks = [|
            0b00000000_00000000_00000000_00011110<rock> // -
            0b00000000_00001000_00011100_00001000<rock> // +
            0b00000000_00000100_00000100_00011100<rock> // J
            0b00010000_00010000_00010000_00010000<rock> // |
            0b00000000_00000000_00011000_00011000<rock> // ::
        |]
        Seq.initInfinite (fun i -> rocks.[i % rocks.Length])

    let inline intersect a (rock : int<rock>) = a &&& int rock <> 0
    let inline shoveLeft (rock : int<rock>) = int rock <<< 1 |> Unit.rock
    let inline shoveRight (rock : int<rock>) = int rock >>> 1 |> Unit.rock

    let bytes (rock : int<rock>) =
        int64 rock // workaround for getting 0 after four right byte shifts
        |> Seq.unfold (fun rockSoFar ->
            if rockSoFar = 0L then None
            else Some (byte rockSoFar |> Unit.level, rockSoFar >>> 8))

module Chamber =

    [<Literal>]
    let LeftWalls = 0b01000000_01000000_01000000_01000000<wall>

    [<Literal>]
    let RightWalls = 0b00000001_00000001_00000001_00000001<wall>

    let create (capacity : int) =
        {JetIdx = 0
         Levels = ResizeArray<byte<level>> capacity
         Height = 0
         Rocks = 0
         Cache = Dictionary ()}

    let levels chamber y =
        if y >= chamber.Levels.Count then 0<block>
        else
            chamber.Levels.GetRange (y, min 4 (chamber.Levels.Count - y))
            |> Seq.rev
            |> Seq.fold (fun blockSoFar level -> (blockSoFar <<< 8) ||| int level) 0
            |> Unit.block

    let skyline chamber y =
        if y <= 0 then 0L<block>
        else
            chamber.Levels.GetRange (max 0 (y - 8), min 8 chamber.Levels.Count)
            |> Seq.rev
            |> Seq.fold (fun blockSoFar level -> (blockSoFar <<< 8) ||| int64 level) 0L
            |> Unit.topBlock

    let toString chamber rockOpt =
        let sb = Text.StringBuilder ()
        
        let rockY, rockLevels =
            rockOpt
            |> Option.map (fun (y, rock) -> y, Rock.bytes rock |> Seq.toArray)
            |> Option.defaultValue (0, Array.empty)

        let top = chamber.Levels.Count + 6
        let bottom = max 0 (chamber.Levels.Count - 30_000)
        {top .. -1 .. bottom}
        |> Seq.iter (fun y ->
            let level = if y < chamber.Levels.Count then byte chamber.Levels.[y] else 0uy
            let rockLevel =
                let y' = y - rockY
                if 0 <= y' && y' < rockLevels.Length then byte rockLevels.[y'] else 0uy

            sb.Append (let s = $"    {y + 1}" in s.Substring (s.Length - 5)) |> ignore
            sb.Append '|' |> ignore

            {6 .. -1 .. 0}
            |> Seq.iter (fun x ->
                let x' = 1uy <<< x
                if rockLevel &&& x' <> 0uy then sb.Append '@'
                elif level &&& x' <> 0uy then sb.Append '#'
                else sb.Append '.'
                |> ignore)

            sb.AppendLine "|" |> ignore)

        if bottom = 0 then sb.AppendLine "     +-------+" |> ignore
        sb.ToString ()

    let shove gust block rock =
        let shovedRock =
            match gust with
            | Right -> if rock |> Rock.intersect (int RightWalls) then rock else Rock.shoveRight rock
            | Left -> if rock |> Rock.intersect (int LeftWalls) then rock else Rock.shoveLeft rock
        if shovedRock |> Rock.intersect (int block) then rock else shovedRock
    
    let putToRest chamber y rock =
        Rock.bytes rock
        |> Seq.iteri (fun i rockLevel ->
            let y' = y + i
            if y' >= chamber.Levels.Count then chamber.Levels.Add rockLevel
            else chamber.Levels.[y'] <- byte chamber.Levels.[y'] ||| byte rockLevel |> Unit.level)

    let dropRock jets targetRocks chamber (rock, rockNumber) =
        let rec loop y jetIdx rockSoFar =
            let nextJetIdx, gust = jets jetIdx
            let block = levels chamber y
            let shovedRock = shove gust block rockSoFar
            let floorOrCollision =
                y <= chamber.Levels.Count &&
                (y = 0 || Rock.intersect (levels chamber (y - 1) |> int) shovedRock)

            if floorOrCollision then
                let topLevels = skyline chamber (y - 1)
                let cacheKey = {JetIdx = jetIdx; Rock = rock; TopLevels = topLevels}
                let replicatedHeight, replicatedRocks =
                    match chamber.Cache.TryGetValue cacheKey with
                    | true, v ->
                        chamber.Cache.Clear ()
                        let patternHeightDelta = chamber.Height - v.Height
                        let patternRocksDelta = rockNumber - v.RocksAdded
                        let rocksRemaining = targetRocks - rockNumber
                        let patternReplicas = rocksRemaining / patternRocksDelta
                        patternReplicas * patternHeightDelta, patternReplicas * patternRocksDelta
                    | _ -> 0L, 0L
                if topLevels <> 0L<block> then
                    chamber.Cache.[cacheKey] <- {Height = chamber.Levels.Count; RocksAdded = rockNumber}
                
                let previousHeight = chamber.Levels.Count
                putToRest chamber y shovedRock
                let heightDelta = chamber.Levels.Count - previousHeight
                {chamber with
                    JetIdx = nextJetIdx
                    Height = chamber.Height + int64 heightDelta + replicatedHeight
                    Rocks = chamber.Rocks + 1L + replicatedRocks}
            else loop (y - 1) nextJetIdx shovedRock

        loop (chamber.Levels.Count + 3) chamber.JetIdx rock

module Api =
    let sw = Stopwatch.StartNew ()
    
    let jets =
        File.ReadAllText "input.txt"
        |> Jet.parse

    let part1 =
        let chamber = Chamber.create (2022 * 4)
        let targetRocks = 2022L

        {1L..targetRocks}
        |> Seq.zip Rock.shapes
        |> Seq.fold (Chamber.dropRock jets targetRocks) chamber

    printfn "Part 1: %i - %f ms" part1.Levels.Count sw.Elapsed.TotalMilliseconds
    sw.Restart ()

    let part2 =
        let chamber = Chamber.create (2022 * 4)
        let targetRocks = 1_000_000_000_000L

        {1L..targetRocks}
        |> Seq.zip Rock.shapes
        |> Seq.scan (Chamber.dropRock jets targetRocks) chamber
        |> Seq.takeWhile (fun c -> c.Rocks <= targetRocks)
        |> Seq.last
        
    printfn "Part 2: %i %f ms" part2.Height sw.Elapsed.TotalMilliseconds
