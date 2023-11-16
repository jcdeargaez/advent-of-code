open System.IO

type Position = {
    X : int
    Y : int
    Height : int
}

type Matrix<'a> = 'a array array

type HeightMap = {
    Height : int
    Width : int
    Items : Position Matrix
}

module Matrix =
    let inline create rows cols value = Array.init rows (fun _ -> Array.replicate cols value)
    let inline item row col (matrix : 'a Matrix) = matrix.[row].[col]
    let inline setItem row col value (matrix : 'a Matrix) = matrix.[row].[col] <- value

module HeightMap =
    let create positions = { Items = positions; Height = positions.Length; Width = positions.[0].Length }

    let findPosition height heightMap =
        heightMap.Items
        |> Seq.collect id
        |> Seq.find (fun pos -> pos.Height = height)

    let startingPosition = findPosition (int 'a' - 1)
    let endingPosition = findPosition (int 'z' + 1)

    let adjacentPositions heightMap position =
        seq {
            if 0 < position.Y then
                yield heightMap.Items |> Matrix.item (position.Y - 1) position.X
            if 0 < position.X then
                yield heightMap.Items |> Matrix.item position.Y (position.X - 1)
            if position.Y + 1 < heightMap.Height then
                yield heightMap.Items |> Matrix.item (position.Y + 1) position.X
            if position.X + 1 < heightMap.Width then
                yield heightMap.Items |> Matrix.item position.Y (position.X + 1)
        }

    let computeSteps heightMap neighborFilter startPosition =
        let visited = Matrix.create heightMap.Height heightMap.Width false
        let steps = Matrix.create heightMap.Height heightMap.Width System.UInt32.MaxValue
        let queue = ResizeArray<Position> (heightMap.Height * heightMap.Width)
        steps |> Matrix.setItem startPosition.Y startPosition.X 0u

        let rec compute () =
            if queue.Count > 0 then
                let position = queue.[0]
                queue.RemoveAt 0
                if visited |> Matrix.item position.Y position.X |> not then
                    let currentSteps = steps |> Matrix.item position.Y position.X
                    let adjacentSteps = currentSteps + 1u
                    adjacentPositions heightMap position
                    |> Seq.filter (neighborFilter position)
                    |> Seq.iter (fun pos ->
                        if adjacentSteps < Matrix.item steps pos.Y pos.X then
                            steps |> Matrix.setItem pos.Y pos.X adjacentSteps
                            queue.Add pos)
                    visited |> Matrix.setItem position.Y position.X true
                compute ()

        queue.Add startPosition
        compute ()
        steps

module Operations =
    let part1 heightMap =
        let filter (position : Position) (neighbor : Position) = neighbor.Height <= position.Height + 1
        let endPosition = HeightMap.endingPosition heightMap
        HeightMap.startingPosition heightMap
        |> HeightMap.computeSteps heightMap filter
        |> Matrix.item endPosition.Y endPosition.X
    
    let part2 heightMap =
        let filter (position : Position) (neighbor : Position) = neighbor.Height >= position.Height - 1
        let steps =
            HeightMap.endingPosition heightMap
            |> HeightMap.computeSteps heightMap filter
        heightMap.Items
        |> Seq.collect id
        |> Seq.filter (fun pos -> pos.Height <= int 'a')
        |> Seq.map (fun pos -> steps |> Matrix.item pos.Y pos.X)
        |> Seq.min

let parseHeight =
    function
    | 'S' -> int 'a' - 1
    | 'E' -> int 'z' + 1
    | x -> int x

let parseLine lineNumber line =
    line
    |> Seq.mapi (fun i ch -> { X = i; Y = lineNumber; Height = parseHeight ch })
    |> Seq.toArray

let heightMap =
    File.ReadAllLines "input.txt"
    |> Array.mapi parseLine
    |> HeightMap.create

Operations.part1 heightMap
|> printfn "Part 1: %i"

Operations.part2 heightMap
|> printfn "Part 2: %i"
