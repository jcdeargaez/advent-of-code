module Simulation

open Domain

let run rounds input =
    let rec runRound i resultsSoFar directions grove =
        if i = rounds then List.rev resultsSoFar
        else
            let result = Round.run grove directions
            let newResults = result :: resultsSoFar
            let newDirections = Direction.cycle directions
            runRound (i + 1) newResults newDirections result.Grove
    
    let rounds =
        input
        |> Grove.parse
        |> runRound 0 List.empty Direction.initial
    
    {Rounds = rounds}
