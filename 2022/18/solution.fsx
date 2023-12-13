#r "nuget: FParsec"

open System
open System.IO

[<Struct>]
type Point = {
    X : int
    Y : int
    Z : int
}

[<Struct>]
type BoundingBox = {
    MinX : int
    MaxX : int
    MinY : int
    MaxY : int
    MinZ : int
    MaxZ : int
}

module Parser =
    open FParsec

    let parse input =
        let pcomma = skipChar ','
        let pnumberComma = pint32 .>> pcomma
        let ppoint =
            tuple3 pnumberComma pnumberComma pint32
            |>> fun (x, y, z) -> { X = x; Y = y; Z = z }
        let ppointLine = ppoint .>> skipNewline
        let ppointLineEof = many1 ppointLine .>> eof        
        match input |> run ppointLineEof with
        | Success (points, _, _) -> points
        | Failure (errorMsg, _, _) -> failwith errorMsg

module Point =
    let neighbors p =
        seq {
            { p with X = p.X + 1 }
            { p with X = p.X - 1 }
            { p with Y = p.Y + 1 }
            { p with Y = p.Y - 1 }
            { p with Z = p.Z + 1 }
            { p with Z = p.Z - 1 }
        }

module BoundingBox =
    let create points =
        let initialBoundingBox = {
            MinX = Int32.MaxValue; MaxX = Int32.MinValue
            MinY = Int32.MaxValue; MaxY = Int32.MinValue
            MinZ = Int32.MaxValue; MaxZ = Int32.MinValue
        }
        let boundingBox =
            points
            |> Seq.fold (fun boundingBoxSoFar point ->
                let boundingBoxSoFar' = {
                    MinX = min boundingBoxSoFar.MinX (point.X - 1)
                    MaxX = max boundingBoxSoFar.MaxX (point.X + 1)
                    MinY = min boundingBoxSoFar.MinY (point.Y - 1)
                    MaxY = max boundingBoxSoFar.MaxY (point.Y + 1)
                    MinZ = min boundingBoxSoFar.MinZ (point.Z - 1)
                    MaxZ = max boundingBoxSoFar.MaxZ (point.Z + 1)
                }
                boundingBoxSoFar') initialBoundingBox
        boundingBox

    let inBoundingBox b p =
           b.MinX <= p.X && p.X <= b.MaxX
        && b.MinY <= p.Y && p.Y <= b.MaxY
        && b.MinZ <= p.Z && p.Z <= b.MaxZ

let input =
    File.ReadAllText "input.txt"
    |> Parser.parse

let allPoints = Set input

let part1 =
    input
    |> Seq.collect Point.neighbors
    |> Seq.filter (allPoints.Contains >> not)
    |> Seq.length

let part2 =
    let boundingBox = BoundingBox.create input

    let rec traverseVolume sidesSoFar  =
        function
        | [] -> sidesSoFar
        | p :: ps ->
            let sidesSoFar' =
                let neighborSides =
                    Point.neighbors p
                    |> Seq.filter allPoints.Contains
                    |> Seq.length
                sidesSoFar
                |> Map.add p neighborSides
            
            let nextPoints =
                Point.neighbors p
                |> Seq.filter (BoundingBox.inBoundingBox boundingBox)
                |> Seq.filter (allPoints.Contains >> not)
                |> Seq.filter (sidesSoFar.ContainsKey >> not)
                |> Seq.toList
            
            nextPoints @ ps
            |> traverseVolume sidesSoFar'
    
    let sides =
        List.singleton { X = boundingBox.MinX; Y = boundingBox.MinY; Z = boundingBox.MinZ }
        |> traverseVolume Map.empty
    
    sides
    |> Map.values
    |> Seq.sum

printfn "Part 1: %i" part1
printfn "Part 2: %i" part2
