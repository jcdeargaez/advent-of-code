module Direction

open Domain

let initial = [North; South; West; East]

let cycle = function
    | h :: t -> t @ [h]
    | [] -> failwith "Empty directions"
