namespace Domain

open System.Collections.Generic

type Direction =
    | North
    | East
    | South
    | West

type Tile = {
    X : int
    Y : int
}

type Elves = HashSet<Tile>

type MoveProposal = {
    Source : Tile
    Target : Tile
}

type Action =
    | Stay of Tile
    | Move of MoveProposal

type Round = {
    Elves : Elves
    Moves : int
}
