#r "nuget:FParsec"

open System
open System.IO

open FParsec

type Point = {
    X : int
    Y : int
}

type Sensor = {
    Position : Point
    ClosestBeacon : Point
    BeaconDistance : int
}

type HorizontalRange = {
    Start : int
    End : int
}

type LineData = {
    Range : HorizontalRange
    CoveredRanges : HorizontalRange list
    Sensors : int
    Beacons : int
}

module HorizontalRange =
    let create start ending =
        if start > ending then failwith "Invalid range"
        { Start = start; End = ending }
    
    let contains x range =
        range.Start <= x && x <= range.End
    
    let overlap a b =
        a.End >= b.Start && a.Start <= b.End

    let merge a b =
        if overlap a b then
            let minStart = min a.Start b.Start
            let maxEnd = max a.End b.End
            create minStart maxEnd |> Some
        else None

module Sensor =
    let create sensor beacon =
        let distance = abs (sensor.X - beacon.X) + abs (sensor.Y - beacon.Y)
        { Position = sensor; ClosestBeacon = beacon; BeaconDistance = distance }
    
    let coverageAt y sensor =
        let distance = sensor.Position.Y - y |> abs
        let remainingDistance = sensor.BeaconDistance - distance
        if remainingDistance < 0 then None
        else
            let range = HorizontalRange.create (sensor.Position.X - remainingDistance) (sensor.Position.X + remainingDistance)
            Some range

module Parser =
    let ppoint : Parser<Point, unit> =
        skipString "x=" >>. pint32 .>> skipString ", y=" .>>. pint32
        |>> fun (x, y) -> { X = x; Y = y }

    let psensor =
        skipString "Sensor at " >>. ppoint .>> skipString ": closest beacon is at " .>>. ppoint
        |>> (<||) Sensor.create

    let psensorLine = psensor .>> skipNewline
    let psensorLinesEof = many psensorLine .>> eof

    let parse input =
        match input |> run psensorLinesEof with
        | Success (sensors, _, _) -> sensors
        | Failure (errorMsg, _, _) -> failwith errorMsg

module Operations =
    let computeY y sensors =
        let lineRanges, lineSensors, lineBeacons, minX, maxX =
            sensors
            |> Seq.fold (fun (rangesSoFar, sensorsSoFar, beaconsSoFar, minXSoFar, maxXSoFar) sensor ->
                let sensorsSoFar' =
                    if sensor.Position.Y <> y then sensorsSoFar
                    else sensorsSoFar |> Set.add sensor.Position.X
                let beaconsSoFar' =
                    if sensor.ClosestBeacon.Y <> y then beaconsSoFar
                    else beaconsSoFar |> Set.add sensor.ClosestBeacon.X
                let rangesSoFar', minXSoFar', maxXSoFar' =
                    match sensor |> Sensor.coverageAt y with
                    | Some range -> range :: rangesSoFar, min range.Start minXSoFar, max range.End maxXSoFar
                    | None -> rangesSoFar, minXSoFar, maxXSoFar
                rangesSoFar', sensorsSoFar', beaconsSoFar', minXSoFar', maxXSoFar') (List.empty, Set.empty, Set.empty, Int32.MaxValue, Int32.MinValue)
        
        let lineData = {
            CoveredRanges = lineRanges
            Range = HorizontalRange.create minX maxX
            Sensors = lineSensors.Count
            Beacons = lineBeacons.Count
        }
        
        lineData

    let part1 y sensors =
        let lineData = sensors |> computeY y
        let lineCoverage =
            {lineData.Range.Start..lineData.Range.End}
            |> Seq.filter (fun x ->
                lineData.CoveredRanges
                |> List.exists (HorizontalRange.contains x))
            |> Seq.length
        
        lineCoverage - lineData.Sensors - lineData.Beacons

    let part2 limit sensors =
        let findUnmergedRange y =
            let lineData = sensors |> computeY y
            let mergedRange =
                lineData.CoveredRanges
                |> Seq.filter (fun r -> r.End > 0 && r.Start < limit)
                |> Seq.sortBy (fun r -> r.Start)
                |> Seq.scan (fun leftRange range ->
                    leftRange
                    |> Option.bind (HorizontalRange.merge range)
                ) (Some (HorizontalRange.create 0 0))
                |> Seq.takeWhile Option.isSome
                |> Seq.last
            
            mergedRange
            |> Option.filter (fun mr -> mr.End < limit)
            |> Option.map (fun mr -> mr.End + 1, y)

        let x, y =
            {0..limit}
            |> Seq.pick findUnmergedRange
        
        uint64 x * 4_000_000UL + uint64 y

let input =
    File.ReadAllText "input.txt"
    |> Parser.parse

input
|> Operations.part1 2_000_000
|> printfn "Part 1: %i"

input
|> Operations.part2 4_000_000
|> printfn "Part 2: %i"
