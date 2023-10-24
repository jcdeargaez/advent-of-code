open System.IO

let top n items =
    let topDesc = ResizeArray<int> (n + 1)

    let updateTop v =
        let i =
            topDesc
            |> Seq.takeWhile ((<) v)
            |> Seq.length
        topDesc.Insert (i, v)
        if topDesc.Count > n then
            topDesc.RemoveAt (topDesc.Count - 1)
    
    items
    |> Seq.iter updateTop

    topDesc.ToArray ()

let add (input : string) =
    async {
        let total =
            input.Split '\n'
            |> Seq.map int
            |> Seq.sum
        return total
    }

let solve topN (input : string) =
    let totals =
        input.Split "\n\n"
        |> Seq.map add
        |> Async.Parallel
        |> Async.RunSynchronously
    
    totals
    |> top topN
    |> Seq.sum

let input =
    "input.txt"
    |> File.ReadAllText

solve 1 input
|> printfn "Part 1: %i"

solve 3 input
|> printfn "Part 2: %i"
