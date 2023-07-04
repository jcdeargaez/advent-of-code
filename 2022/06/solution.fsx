open System.IO

let marker chars len =
    chars
    |> Seq.windowed len
    |> Seq.findIndex (Set >> Set.toSeq >> Seq.length >> (=) len)
    |> (+) len

let input = File.ReadAllText "input.txt"

[4; 14]
|> Seq.map (marker input)
|> Seq.iteri (fun i p -> printfn "Part %i: %i" (i + 1) p)
