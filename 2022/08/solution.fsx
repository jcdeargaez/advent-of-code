open System.IO

let input =
    File.ReadAllLines "input.txt"
    |> Array.map (fun line ->
        line.ToCharArray ()
        |> Array.map (string >> int))

let vertically f =
    let m = Array.init input.Length (fun _ -> Array.replicate input.[0].Length 0)
    input.[0]
    |> Array.iteri (fun j _ ->
        let column = 
            input
            |> Array.map (Array.item j)
        f column
        |> Array.iteri (fun i v -> m.[i].[j] <- v))
    m

let horizontally f = Array.init input.Length (fun i -> f input.[i])

let part1 =
    let scanMax = Array.scan max 0 >> Array.tail
    let scanBackMax items =
        Array.scanBack max items 0
        |> Array.take items.Length

    let up = vertically scanMax
    let left = horizontally scanMax
    let down = vertically scanBackMax
    let right = horizontally scanBackMax

    let visible i j =
        let h = input.[i].[j]
        h > up.[i - 1].[j] || h > left.[i].[j - 1] || h > down.[i + 1].[j] || h > right.[i].[j + 1]

    let edge = input.Length * 2 + (input.[0].Length - 2) * 2
    let interior =
        {1..input.Length - 2}
        |> Seq.sumBy (fun i ->
            {1..input.[i].Length - 2}
            |> Seq.filter (fun j -> visible i j)
            |> Seq.length)
    
    edge + interior

let part2 =
    let visibility heights =
        let idxByHeight = Array.replicate 10 0
        heights
        |> Array.mapi (fun i h ->
            let distance = i - (Array.max idxByHeight.[h..])
            idxByHeight.[h] <- i
            distance)
    let visibilityBack = Array.rev >> visibility >> Array.rev

    let up = vertically visibility
    let left = horizontally visibility
    let down = vertically visibilityBack
    let right = horizontally visibilityBack

    let scenicScore i j = up.[i].[j] * left.[i].[j] * down.[i].[j] * right.[i].[j]

    {1..input.Length - 2}
    |> Seq.map (fun i ->
        {1..input.[i].Length - 2}
        |> Seq.map (fun j -> scenicScore i j)
        |> Seq.max)
    |> Seq.max

printfn "Part 1: %i" part1
printfn "Part 2: %i" part2
