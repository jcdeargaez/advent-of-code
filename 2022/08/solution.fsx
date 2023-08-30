open System.IO

let input =
    File.ReadAllLines "input.txt"
    |> Array.map (fun line ->
        line.ToCharArray ()
        |> Array.map (string >> int))

let part1 =
    let up =
        let m = Array.init input.Length (fun i -> Array.replicate input.[i].Length 0)
        input.[0]
        |> Seq.iteri (fun j _ ->
            input
            |> Seq.map (Array.item j)
            |> Seq.scan max 0
            |> Seq.tail
            |> Seq.iteri (fun i v -> m.[i].[j] <- v))
        m

    let left =
        Array.init input.Length (fun i ->
            input.[i]
            |> Array.scan max 0
            |> Array.tail)

    let down =
        let m = Array.init input.Length (fun i -> Array.replicate input.[i].Length 0)
        input.[0]
        |> Seq.iteri (fun j _ ->
            let col =
                input
                |> Seq.map (Array.item j)
            Seq.scanBack max col 0
            |> Seq.take input.Length
            |> Seq.iteri (fun i v -> m.[i].[j] <- v))
        m

    let right =
        Array.init input.Length (fun i ->
            Array.scanBack max input.[i] 0
            |> Array.take input.[i].Length)

    let edge = input.Length * 2 + (input.[0].Length - 2) * 2
    let interior =
        {1..input.Length - 2}
        |> Seq.sumBy (fun i ->
            {1..input.[i].Length - 2}
            |> Seq.filter (fun j ->
                let h = input.[i].[j]
                h > up.[i - 1].[j] || h > left.[i].[j - 1] || h > down.[i + 1].[j] || h > right.[i].[j + 1])
            |> Seq.length)
    
    edge + interior

let part2 =
    let scenicScores heights =
        let indices = Array.replicate 10 0
        heights
        |> Seq.mapi (fun i h ->
            let score = i - (Array.max indices.[h..])
            indices.[h] <- i
            score)
        |> Seq.toArray

    let up =
        let m = Array.init input.Length (fun i -> Array.replicate input.[i].Length 0)
        input.[0]
        |> Seq.iteri (fun j _ ->
            input
            |> Seq.map (Array.item j)
            |> scenicScores
            |> Seq.iteri (fun i v -> m.[i].[j] <- v))
        m

    let left =
        Array.init input.Length (fun i ->
            input.[i]
            |> scenicScores)

    let down =
        let m = Array.init input.Length (fun i -> Array.replicate input.[i].Length 0)
        input.[0]
        |> Seq.iteri (fun j _ ->
            input
            |> Array.map (Array.item j)
            |> Array.rev
            |> scenicScores
            |> Array.rev
            |> Array.iteri (fun i v -> m.[i].[j] <- v))
        m

    let right =
        Array.init input.Length (fun i ->
            input.[i]
            |> Array.rev
            |> scenicScores
            |> Array.rev)

    {1..input.Length - 2}
    |> Seq.map (fun i ->
        {1..input.[i].Length - 2}
        |> Seq.map (fun j -> up.[i].[j] * left.[i].[j] * down.[i].[j] * right.[i].[j])
        |> Seq.max)
    |> Seq.max

printfn "Part 1: %i" part1
printfn "Part 2: %i" part2
