#r "nuget:FParsec"

open System.Collections.Generic
open System.IO

open FParsec

type Point = {
    X : int
    Y : int
}

let [<Literal>] PouringX = 500

let inputParser =
    let ppoint: Parser<Point, unit> =
        pint32 .>> skipChar ',' .>>. pint32
        |>> fun (x, y) -> { X = x; Y = y }
    let parrow: Parser<unit, unit> = skipString " -> "
    let ppath =
        ppoint .>> parrow .>>. sepBy1 ppoint parrow
        |>> fun (p, ps) -> p :: ps
    let ppathline = ppath .>> skipNewline
    let ppathlines = many1 ppathline .>> eof
    ppathlines

let parseInput input =
    match run inputParser input with
    | Success (points, _, _) -> points
    | Failure (err, _, _) -> failwith err

let expandPath p1 p2 =
    let minX, maxX = min p1.X p2.X, max p1.X p2.X
    let minY, maxY = min p1.Y p2.Y, max p1.Y p2.Y
    Seq.allPairs {minX .. maxX} {minY .. maxY}
    |> Seq.map (fun (x, y) -> { X = x; Y = y })

let nextMoves grain =
    seq {
        let y = grain.Y + 1
        yield { grain with Y = y }
        yield { X = grain.X - 1; Y = y }
        yield { X = grain.X + 1; Y = y }
    }

let simulateSand stop freeSpace (cave : Point HashSet) =
    let rec tick i point =
        if stop point then i
        else
            nextMoves point
            |> Seq.tryFind freeSpace
            |> function
                | Some p -> tick i p
                | None ->
                    cave.Add point |> ignore
                    tick (i + 1) { X = PouringX; Y = 0 }

    tick 0 { X = PouringX; Y = 0 }

let cave = HashSet<Point> ()

let baseY, leftX, rightX =
    File.ReadAllText "input.txt"
    |> parseInput
    |> Seq.map Seq.pairwise
    |> Seq.map (Seq.map ((<||) expandPath))
    |> Seq.map (Seq.collect id)
    |> Seq.collect id
    |> Seq.fold (fun (baseYSoFar, leftXSoFar, rightXSoFar) point ->
        cave.Add point |> ignore
        max point.Y baseYSoFar, min point.X leftXSoFar, max point.X rightXSoFar) (0, PouringX, PouringX)

let ans1 =
    simulateSand
        (fun p -> p.X < leftX || p.X > rightX || p.Y > baseY)
        (cave.Contains >> not)
        cave

let ans2 =
    let topGrain = { X = PouringX; Y = 0 }
    let floorY = baseY + 2
    simulateSand
        (fun _ -> cave.Contains topGrain)
        (fun p -> p.Y < floorY && cave.Contains p |> not)
        cave

printfn "Part 1: %i" ans1
printfn "Part 2: %i" (ans1 + ans2)
