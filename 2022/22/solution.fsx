#r "nuget: FParsec"

open System
open System.Collections.Generic
open System.IO

[<Measure>] type Direction
[<Measure>] type Rotation
[<Measure>] type Coordinate

type Position = {
    X : uint<Coordinate>
    Y : uint<Coordinate>
    Direction : uint<Direction>
}

// Row or column
type Bar = {
    Start : uint<Coordinate>
    End : uint<Coordinate>
    Length : uint<Coordinate>
}

type Board = {
    Height : uint<Coordinate>
    Rows : Bar array
    Columns : Bar array
    InitialPosition : Position
    Walls : HashSet<uint<Coordinate> * uint<Coordinate>>
}

type Action =
    | Rotate of int<Rotation>
    | Move of uint<Coordinate>

type Note = {
    Board : Board
    Path : Action list
}

module Direction =
    let [<Literal>] Right = 0u<Direction>
    let [<Literal>] Down = 1u<Direction>
    let [<Literal>] Left = 2u<Direction>
    let [<Literal>] Up = 3u<Direction>
    let [<Literal>] Initial = Right

module Rotation =
    let [<Literal>] Clockwise = 1<Rotation>
    let [<Literal>] CounterClockwise = -1<Rotation>

    let inline rotate (rotation : int<Rotation>) (dir : uint<Direction>) =
        (int dir + int rotation % 4 + 4) % 4
        |> uint |> ( * ) 1u<Direction>

module Position =
    let create x y dir =
        { X = x; Y = y; Direction = dir }
    
    let rotate rotation position =
        let newDirection = Rotation.rotate rotation position.Direction
        { position with Direction = newDirection }

module Coordinate =
    let inline create v =
        if v < 0 then failwith "Invalid value for a coordinate"
        uint v |> ( * ) 1u<Coordinate>

module Board =
    let private tiles = [| '.'; '#' |]

    let private createRow (line : string) =
        if String.IsNullOrWhiteSpace line then failwith "Invalid line"
        let left = line.IndexOfAny tiles |> Coordinate.create
        let right = line.LastIndexOfAny tiles |> Coordinate.create
        let length = right - left + 1u<Coordinate>
        { Start = left; End = right; Length = length }
    
    let private createColumns (lines : string array) =
        {0..lines.[0].Length - 1}
        |> Seq.map (fun col ->
            lines
            |> Array.map (fun line -> if col < line.Length then line.[col] else ' ')
            |> (String >> createRow))
        |> Seq.toArray

    let create lines =
        let rows = lines |> Array.map createRow
        let cols = lines |> createColumns
        let initialPosition =
            let x = lines.[0].IndexOf ('.', int rows.[0].Start) |> Coordinate.create
            let y = 0 |> Coordinate.create
            Position.create x y Direction.Initial
        let height = Coordinate.create rows.Length
        let walls =
            lines
            |> Seq.indexed
            |> Seq.collect (fun (i, line) ->
                line
                |> Seq.mapi (fun j ch -> if ch = '#' then Some (i |> Coordinate.create, j |> Coordinate.create) else None))
            |> Seq.choose id
            |> HashSet
        { Rows = rows; Columns = cols; Height = height; InitialPosition = initialPosition; Walls = walls }

    let inline wall board position =
        board.Walls.Contains (position.Y, position.X)

    let remap length x y dir =
        if length = 4 (* sample file *) then
            match x, y with
            // Face 1
            | x, y when dir = Direction.Right && x = length * 3 && length * 0 <= y && y < length * 1 -> (* Face 6 Left *) { X = length * 4 - 1 |> Coordinate.create; Y = length * 3 - y - 1 |> Coordinate.create; Direction = Direction.Left }
            | x, y when dir = Direction.Left && x = length * 2 - 1 && length * 0 <= y && y < length -> (* Face 3 Down *) { X = length + y |> Coordinate.create ; Y = length |> Coordinate.create; Direction = Direction.Down }
            | x, y when dir = Direction.Up && length * 2 <= x && x < length * 3 && y = length * 0 - 1 -> (* Face 2 Down *) { X = length * 3 - x - 1 |> Coordinate.create; Y = length |> Coordinate.create; Direction = Direction.Down }

            // Face 2
            | x, y when dir = Direction.Left && x = length * 0 - 1 && length * 1 <= y && y < length * 2 -> (* Face 6 Up *) { X = length * 5 - y - 1 |> Coordinate.create; Y = length * 3 - 1 |> Coordinate.create; Direction = Direction.Up }
            | x, y when dir = Direction.Up && length * 0 <= x && x < length * 1 && y = length * 1 - 1 -> (* Face 1 Down *) { X = length * 3 - x - 1 |> Coordinate.create; Y = 0 |> Coordinate.create; Direction = Direction.Down }
            | x, y when dir = Direction.Down && length * 0 <= x && x < length * 1 && y = length * 2 -> (* Face 5 Up *) { X = length * 3 - x - 1 |> Coordinate.create; Y = length * 3 - 1 |> Coordinate.create; Direction = Direction.Up }

            // Face 3
            | x, y when dir = Direction.Up && length * 1 <= x && x < length * 2 && y = length * 1 - 1 -> (* Face 1 Right *) { X = length * 2 |> Coordinate.create; Y = x - length |> Coordinate.create; Direction = Direction.Right }
            | x, y when dir = Direction.Down && length * 1 <= x && x < length * 2 && y = length * 2 -> (* Face 5 Right *) { X = length * 2 |> Coordinate.create; Y = length * 4 - x - 1 |> Coordinate.create; Direction = Direction.Right }

            // Face 4
            | x, y when dir = Direction.Right && x = length * 3 && length * 1 <= y && y < length * 2 -> (* Face 6 Down *) { X = length * 5 - y - 1 |> Coordinate.create; Y = length * 2 |> Coordinate.create; Direction = Direction.Down }

            // Face 5
            | x, y when dir = Direction.Left && x = length * 2 - 1 && length * 2 <= y && y < length * 3 -> (* Face 3 Up *) { X = length * 4 - y - 1 |> Coordinate.create; Y = length * 2 - 1 |> Coordinate.create; Direction = Direction.Up }
            | x, y when dir = Direction.Down && length * 2 <= x && x < length * 3 && y = length * 3 -> (* Face 2 Up *) { X = length * 3 - x - 1 |> Coordinate.create; Y = length * 2 - 1 |> Coordinate.create; Direction = Direction.Up }

            // Face 6
            | x, y when dir = Direction.Right && x = length * 4 && length * 2 <= y && y < length * 3 -> (* Face 1 Left *) { X = length * 3 - 1 |> Coordinate.create; Y = length * 3 - y - 1 |> Coordinate.create; Direction = Direction.Left }
            | x, y when dir = Direction.Up && length * 3 <= x && x < length * 4 && y = length * 2 - 1 -> (* Face 4 Left *) { X = length * 3 - 1 |> Coordinate.create; Y = length * 5 - x - 1 |> Coordinate.create; Direction = Direction.Left }
            | x, y when dir = Direction.Down && length * 3 <= x && x < length * 4 && y = length * 3 -> (* Face 2 Right *) { X = 0 |> Coordinate.create; Y = length * 5 - x - 1 |> Coordinate.create; Direction = Direction.Right }

            | _ -> { X = x |> Coordinate.create; Y = y |> Coordinate.create; Direction = dir }
        else (* input file *)
            match x, y with
            // Face 1
            | x, y when dir = Direction.Left && x = length * 1 - 1 && length * 0 <= y && y < length * 1 -> (* Face 4 Right *) { X = 0 |> Coordinate.create; Y = length * 3 - y - 1 |> Coordinate.create; Direction = Direction.Right }
            | x, y when dir = Direction.Up && length * 1 <= x && x < length * 2 && y = length * 0 - 1 -> (* Face 6 Right *) { X = 0 |> Coordinate.create; Y = length * 2 + x |> Coordinate.create; Direction = Direction.Right }

            // Face 2
            | x, y when dir = Direction.Right && x = length * 3 && length * 0 <= y && y < length * 1 -> (* Face 5 Left *) { X = length * 2 - 1 |> Coordinate.create; Y = length * 3 - y - 1 |> Coordinate.create; Direction = Direction.Left }
            | x, y when dir = Direction.Down && length * 2 <= x && x < length * 3 && y = length * 1 -> (* Face 3 Left *) { X = length * 2 - 1 |> Coordinate.create; Y = x - length |> Coordinate.create; Direction = Direction.Left }
            | x, y when dir = Direction.Up && length * 2 <= x && x < length * 3 && y = length * 0 - 1 -> (* Face 6 Up *) { X = x - length * 2 |> Coordinate.create; Y = length * 4 - 1 |> Coordinate.create; Direction = Direction.Up }

            // Face 3
            | x, y when dir = Direction.Left && x = length * 1 - 1 && length * 1 <= y && y < length * 2 -> (* Face 4 Down *) { X = y - length |> Coordinate.create; Y = length * 2 |> Coordinate.create; Direction = Direction.Down }
            | x, y when dir = Direction.Right && x = length * 2 && length * 1 <= y && y < length * 2 -> (* Face 2 Up *) { X = length + y |> Coordinate.create; Y = length * 1 - 1 |> Coordinate.create; Direction = Direction.Up }

            // Face 4
            | x, y when dir = Direction.Left && x = length * 0 - 1 && length * 2 <= y && y < length * 3 -> (* Face 1 Right *) { X = length |> Coordinate.create; Y = length * 3 - y - 1 |> Coordinate.create; Direction = Direction.Right }
            | x, y when dir = Direction.Up && length * 0 <= x && x < length * 1 && y = length * 2 - 1 -> (* Face 3 Right *) { X = length * 1 |> Coordinate.create; Y = x + length |> Coordinate.create; Direction = Direction.Right }

            // Face 5
            | x, y when dir = Direction.Right && x = length * 2 && length * 2 <= y && y < length * 3 -> (* Face 2 Left *) { X = length * 3 - 1 |> Coordinate.create; Y = length * 3 - y - 1 |> Coordinate.create; Direction = Direction.Left }
            | x, y when dir = Direction.Down && length * 1 <= x && x < length * 2 && y = length * 3 -> (* Face 6 Left *) { X = length * 1 - 1 |> Coordinate.create; Y = length * 2 + x |> Coordinate.create; Direction = Direction.Left }

            // Face 6
            | x, y when dir = Direction.Left && x = length * 0 - 1 && length * 3 <= y && y < length * 4 -> (* Face 1 Down *) { X = y - length * 2 |> Coordinate.create; Y = 0 |> Coordinate.create; Direction = Direction.Down }
            | x, y when dir = Direction.Down && length * 0 <= x && x < length * 1 && y = length * 4 -> (* Face 2 Down *) { X = x + length * 2 |> Coordinate.create; Y = 0 |> Coordinate.create; Direction = Direction.Down }
            | x, y when dir = Direction.Right && x = length * 1 && length * 3 <= y && y < length * 4 -> (* Face 5 Up *) { X = y - length * 2 |> Coordinate.create; Y = length * 3 - 1 |> Coordinate.create; Direction = Direction.Up }

            | _ -> { X = x |> Coordinate.create; Y = y |> Coordinate.create; Direction = dir }

module Action =
    open FParsec

    let private dx = [| 1; 0; -1; 0 |]
    let private dy = [| 0; 1; 0; -1 |]

    let parse input =
        let pmove =
            let pnonZeroDigit = anyOf [|'1'..'9'|]
            let pnumberGT0 = pnonZeroDigit .>>. manySatisfy isDigit |>> fun (d, ds) -> $"{d}{ds}"
            pnumberGT0 |>> (int >> Coordinate.create >> Move)
        let protate = choice [
            charReturn 'R' (Rotate Rotation.Clockwise)
            charReturn 'L' (Rotate Rotation.CounterClockwise)
        ]
        let paction = pmove <|> protate
        let ppath = many1 paction
        let ppathEof = ppath .>> eof
        match input |> run ppathEof with
        | Success (actions, _, _) -> actions
        | Failure (errorMsg, _, _) -> failwith errorMsg

    let forward faceLength board position =
        let x = int position.X + dx.[int position.Direction]
        let y = int position.Y + dy.[int position.Direction]

        faceLength
        |> Option.map (fun length -> Board.remap length x y position.Direction)
        |> Option.defaultWith (fun () ->
            if position.Direction = Direction.Left || position.Direction = Direction.Right then
                let row = board.Rows.[y]
                { position with
                    X = if x < int row.Start then row.End
                        elif x > int row.End then row.Start
                        else x |> Coordinate.create }
            else
                let col = board.Columns.[x]
                { position with
                    Y = if y < int col.Start then col.End
                        elif y > int col.End then col.Start
                        else y |> Coordinate.create })

    let move faceLength board tiles position =
        let rec next i positionSoFar =
            if i = tiles then positionSoFar
            else
                let newPosition = forward faceLength board positionSoFar
                if Board.wall board newPosition then positionSoFar
                else newPosition |> next (i + 1)
        
        position |> next 0

    let follow faceLength board position =
        function
        | Move n -> position |> move faceLength board (int n)
        | Rotate r -> position |> Position.rotate r

module Note =
    let create (lines : string array) =
        let pathLine = lines.[lines.Length - 1]
        let boardLines = lines.[0..lines.Length - 3]
        let board = Board.create boardLines
        let path = Action.parse pathLine
        { Board = board; Path = path }

let password position =
    1000 * (int position.Y + 1) + 4 * (int position.X + 1) + int position.Direction

let follow faceLength note =
    note.Path
    |> List.fold (fun positionSoFar action ->
        Action.follow faceLength note.Board positionSoFar action) note.Board.InitialPosition
    |> password

let part1 note =
    note
    |> follow None

let part2 note =
    note
    |> follow (Some 50)

let note =
    File.ReadAllLines "input.txt"
    |> Note.create

part1 note
|> printfn "Part 1: %i"

part2 note
|> printfn "Part 2: %i"
