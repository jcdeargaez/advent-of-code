module Action

open Domain

let compute grove tile directions =
    let rec move i =
        if i = List.length directions then Stay tile
        else
            let direction = directions.[i]
            if Grove.areFrontTilesFree grove tile direction then
                let frontTile =
                    match direction with
                    | North -> {tile with Y = tile.Y - 1}
                    | East  -> {tile with X = tile.X + 1}
                    | West  -> {tile with X = tile.X - 1}
                    | South -> {tile with Y = tile.Y + 1}
                Move {Source = tile; Target = frontTile; Direction = direction}
            else move (i + 1)
    
    if Grove.isElfAlone grove tile then Stay tile else move 0
