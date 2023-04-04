namespace Domain

type Direction =
    | North
    | East
    | South
    | West

type Tile = {
    X : int
    Y : int
}

type Grove = {
    Width : int
    Height : int
    Elves : Tile Set
}

type GroveRectangle = {
    NWTile : Tile
    SETile : Tile
}

type MoveProposal = {
    Source : Tile
    Target : Tile
    Direction : Direction
}

type Action =
    | Stay of Tile
    | Move of MoveProposal

type Round = {
    Directions : Direction List
    Actions : Action List
    Collisions : Tile Set
    Grove : Grove
    MinimumGroveRectangle : GroveRectangle
}

type Simulation =  {
    Rounds : Round List
} 
