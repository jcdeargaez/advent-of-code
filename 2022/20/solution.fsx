open System.IO

let mix (items : int64 array) (indices : int ResizeArray) =
    let count = indices.Count - 1 |> int64
    for i in 0..indices.Count - 1 do
        let j = indices.IndexOf i
        indices.RemoveAt j
        let k = ((int64 j + items.[i]) % count + count) % count
        indices.Insert (int k, i)

let sumIndices (items : int64 array) (indices : int ResizeArray) =
    let i0 = items |> Array.findIndex ((=) 0) |> indices.IndexOf
    {1..3} |> Seq.sumBy (fun i -> items.[indices.[(i0+i*1000) % indices.Count]])

let input = File.ReadLines "input.txt" |> Seq.map int64 |> Seq.toArray

let part1 =
    let indices = ResizeArray {0..input.Length - 1}
    mix input indices
    sumIndices input indices

printfn "Part 1: %i" part1

let part2 =
    let keyInput = input |> Array.map (( * ) 811589153L)
    let indices = ResizeArray {0..input.Length - 1}
    for _ in 1..10 do
        mix keyInput indices
    sumIndices keyInput indices

printfn "Part 2: %i" part2
