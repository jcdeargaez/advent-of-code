module Grove

open Domain

let parse lines =
    let processLine stateSoFar (row, line) =
        let (_, widthSoFar, elvesSoFar) = stateSoFar
        let lineElves =
            line
            |> Seq.indexed
            |> Seq.choose (fun (col, c) ->
                match c with
                | '#' -> Some {X = col; Y = row}
                |  _  -> None)
            |> Set
        row, max widthSoFar (String.length line), Set.union elvesSoFar lineElves

    let (height, width, elves) =
        lines
        |> Seq.indexed
        |> Seq.fold processLine (0, 0, Set.empty)

    {Width = width; Height = height; Elves = elves}

let toString grove =
    let sb = System.Text.StringBuilder (grove.Width * grove.Height)
    {0..grove.Height - 1}
    |> Seq.iter (fun row ->
        {0..grove.Width - 1}
        |> Seq.iter (fun col ->
            let c = if grove.Elves.Contains {X = col; Y = row} then '#' else '.'
            sb.Append c |> ignore)
        sb.AppendLine () |> ignore)
    sb.ToString ()

let private frontVisibleTiles grove tile direction =
    let frontTiles =
        match direction with
        | North ->
            let y = tile.Y - 1
            [{X = tile.X - 1; Y = y}; {tile with Y = y}; {X = tile.X + 1; Y = y}]
        | East ->
            let x = tile.X + 1
            [{X = x; Y = tile.Y - 1}; {tile with X = x}; {X = x; Y = tile.Y + 1}]
        | South ->
            let y = tile.Y + 1
            [{X = tile.X - 1; Y = y}; {tile with Y = y}; {X = tile.X + 1; Y = y}]
        | West ->
            let x = tile.X - 1
            [{X = x; Y = tile.Y - 1}; {tile with X = x}; {X = x; Y = tile.Y + 1}]
    
    frontTiles
    |> List.filter (fun t -> 0 <= t.X && t.X < grove.Width && 0 <= t.Y && t.Y < grove.Height)

let isElfAlone grove tile =
    let surroundingTiles =
        Direction.initial
        |> List.collect (frontVisibleTiles grove tile)
        |> Set

    grove.Elves
    |> Set.intersect surroundingTiles
    |> Set.isEmpty

let areFrontTilesFree grove tile direction =
    let frontTiles = frontVisibleTiles grove tile direction
    if frontTiles |> List.isEmpty then false
    else
        grove.Elves
        |> Set.intersect (Set frontTiles)
        |> Set.isEmpty

let computeMinimumRectangle grove =
    let computeMinMaxCoords stateSoFar tile =
        let ({X = w; Y = n}, {X = e; Y = s}) = stateSoFar
        let nwTile = {X = min w tile.X; Y = min n tile.Y}
        let seTile = {X = max e tile.X; Y = max s tile.Y}
        nwTile, seTile

    let (nwTile, seTile) =
        grove.Elves
        |> Set.fold computeMinMaxCoords ({X = grove.Width; Y = grove.Height}, {X = 0; Y = 0})
    
    {NWTile = nwTile; SETile = seTile}
