open System.IO

open Domain

[<EntryPoint>]
let main _ =
    let simulation =
        File.ReadLines "input.txt"
        |> Simulation.run 20

    simulation.Rounds
    |> Seq.indexed
    |> Seq.iter (fun (i, r) ->
        printfn "Round %i" (i + 1)

        Grove.toString r.Grove
        |> printfn "%s"
        
        let minRectArea = (r.MinimumGroveRectangle.SETile.X - r.MinimumGroveRectangle.NWTile.X) * (r.MinimumGroveRectangle.SETile.Y - r.MinimumGroveRectangle.NWTile.Y)
        let moves =
            r.Actions
            |> List.sumBy (function | Move _ -> 1 | _ -> 0)
        
        printfn "Moves: %i" moves
        printfn "Min rectangle area: %i" minRectArea
        printfn "")

    0
