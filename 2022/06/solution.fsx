open System.Collections.Generic
open System.IO

/// Simple but not with the best performance. Needs to build the set from scratch for every window.
/// Finds the answer when the set length is equal to len, meaning all chars are unique.
let marker1 msg len =
    msg
    |> Seq.windowed len
    |> Seq.findIndex (Set >> Set.toSeq >> Seq.length >> (=) len)
    |> (+) len

/// Not so simple but with improved performance. Adds the next char and removes the old to avoid rebuilding a set.
/// Finds the marker when the frequencies dictionary length is equal to len, meaning all chars are unique having frequency of 1.
let marker2 (msg : string) len =
    /// Dictionary preferred over Map due to performance.
    let frequencies =
        msg.[..len - 1]
        |> Seq.countBy id
        |> Seq.map KeyValuePair
        |> Dictionary

    let rec inner i =
        if frequencies.Count = len then len + i
        elif len + i < msg.Length then
            let newChar = msg.[len + i]
            match frequencies.TryGetValue newChar with
            | true, f -> frequencies.[newChar] <- f + 1
            | _ -> frequencies.[newChar] <- 1

            let oldChar = msg.[i]
            match frequencies.[oldChar] with
            | 1 -> frequencies.Remove oldChar |> ignore
            | f -> frequencies.[oldChar] <- f - 1

            inner (i + 1)
        else failwith "Didn't find a message marker"

    inner 0

let input = File.ReadAllText "input.txt"

[4; 14]
|> Seq.map (marker2 input)
|> Seq.iteri (fun i p -> printfn "Part %i: %i" (i + 1) p)
