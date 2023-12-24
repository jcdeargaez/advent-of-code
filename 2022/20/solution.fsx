open System.IO

let mix (input : ResizeArray<int * int64>) =
    let move i =
        while fst input.[0] <> i do
            let n = input.[0]
            input.RemoveAt 0
            input.Add n

        let item = input.[0]
        input.RemoveAt 0

        {0L .. (snd item |> abs) % int64 input.Count - 1L}
        |> Seq.iter (fun _ ->
            if snd item > 0 then
                let n = input.[0]
                input.RemoveAt 0
                input.Add n
            else
                let last = input.Count - 1
                let n = input.[last]
                input.RemoveAt last
                input.Insert (0, n))
        
        if snd item >= 0 then input.Add item
        else input.Insert (0, item)

    {0..input.Count - 1}
    |> Seq.iter move

let sumIndices (items : ResizeArray<int * int64>) =
    let i0 =
        items
        |> Seq.findIndex (snd >> (=) 0)
    
    {1..3}
    |> Seq.sumBy (fun i -> snd items.[(i * 1000 + i0) % items.Count])

let input =
    File.ReadAllLines "input.txt"
    |> Array.map int64
    |> Array.indexed

let part1 =
    let items = ResizeArray input
    mix items
    sumIndices items

printfn "Part 1: %i" part1

let part2 =
    let items =
        input
        |> Seq.map (fun (i, v) -> i, v * 811589153L)
        |> ResizeArray

    {1..10}
    |> Seq.iter (fun _ -> mix items)
    sumIndices items

printfn "Part 2: %i" part2
