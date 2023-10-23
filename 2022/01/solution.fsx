open System.IO

let solve topN input =
    let topDesc = ResizeArray<int> (topN + 1)

    let updateTop v =
        let i =
            topDesc
            |> Seq.takeWhile ((<) v)
            |> Seq.length
        topDesc.Insert (i, v)
        if topDesc.Count > topN then
            topDesc.RemoveAt (topDesc.Count - 1)

    let processLine caloriesSoFar line =
        if String.length line <> 0 then
            caloriesSoFar + int line
        else
            updateTop caloriesSoFar
            0
    
    let lastCalories =
        input
        |> Seq.fold processLine 0
    
    if lastCalories > 0 then
        updateTop lastCalories
    
    topDesc
    |> Seq.sum

let input =
    "sample.txt"
    |> File.ReadLines

printfn "Part 1 - %i" (solve 1 input)
printfn "Part 2 - %i" (solve 3 input)
