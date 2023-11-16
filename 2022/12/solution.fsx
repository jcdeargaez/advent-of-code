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

    let inline item (matrix : 'a Matrix) row col = matrix.[row].[col]

    let inline setItem (matrix : 'a Matrix) row col value = matrix.[row].[col] <- value

module HeightMap =
    let create positions = { Items = positions; Height = positions.Length; Width = positions.[0].Length }

module Operations =
    let findPosition height heightMap =
        heightMap.Items
        |> Seq.collect id
        |> Seq.find (fun pos -> pos.Height = height)

    let startingPosition = findPosition  (int 'a' - 1)
    
    let endingPosition = findPosition  (int 'z' + 1)

    let adjacentPositions heightMap position =
        seq {
            if 0 < position.Y then
                yield Matrix.item heightMap.Items (position.Y - 1) position.X
            if 0 < position.X then
                yield Matrix.item heightMap.Items position.Y (position.X - 1)
            if position.Y + 1 < heightMap.Height then
                yield Matrix.item heightMap.Items (position.Y + 1) position.X
            if position.X + 1 < heightMap.Width then
                yield Matrix.item heightMap.Items position.Y (position.X + 1)
        }

    let computeSteps heightMap neighborFilter startPosition =
        let visited = Matrix.create heightMap.Height heightMap.Width false
        let steps = Matrix.create heightMap.Height heightMap.Width System.UInt32.MaxValue
        let queue = ResizeArray<Position> (heightMap.Height * heightMap.Width)
        Matrix.setItem steps startPosition.Y startPosition.X 0u

        let rec compute () =
            if queue.Count > 0 then
                let position = queue.[0]
                queue.RemoveAt 0
                if Matrix.item visited position.Y position.X |> not then
                    let currentSteps = Matrix.item steps position.Y position.X
                    let adjacentSteps = currentSteps + 1u
                    adjacentPositions heightMap position
                    |> Seq.filter (neighborFilter position)
                    |> Seq.iter (fun pos ->
                        if adjacentSteps < steps.[pos.Y].[pos.X] then
                            steps.[pos.Y].[pos.X] <- adjacentSteps
                            queue.Add pos)
                    Matrix.setItem visited position.Y position.X true
                compute ()

        queue.Add startPosition
        compute ()
        steps

    let part1 heightMap =
        let filter (position : Position) (neighbor : Position) = neighbor.Height <= position.Height + 1
        let steps =
            startingPosition heightMap
            |> computeSteps heightMap filter
        let endPosition = endingPosition heightMap
        Matrix.item steps endPosition.Y endPosition.X
    
    let part2 heightMap =
        let filter (position : Position) (neighbor : Position) = neighbor.Height >= position.Height - 1
        let steps =
            endingPosition heightMap
            |> computeSteps heightMap filter

        heightMap.Items
        |> Seq.collect id
        |> Seq.filter (fun pos -> pos.Height <= int 'a')
        |> Seq.map (fun pos -> Matrix.item steps pos.Y pos.X)
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
