namespace Domain

open System.Collections.Generic

type Tile = {
    X : int
    Y : int
}

type Direction = {
    FrontLeft  : Tile -> Tile
    Front      : Tile -> Tile
    FrontRight : Tile -> Tile
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
