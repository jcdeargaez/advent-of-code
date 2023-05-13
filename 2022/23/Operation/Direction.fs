module Direction

open Domain

let inline north tile = {tile with Y = tile.Y - 1}
let inline south tile = {tile with Y = tile.Y + 1}
let inline west tile  = {tile with X = tile.X - 1}
let inline east tile  = {tile with X = tile.X + 1}
let inline northWest tile = {X = tile.X - 1; Y = tile.Y - 1} 
let inline northEast tile = {X = tile.X + 1; Y = tile.Y - 1} 
let inline southWest tile = {X = tile.X - 1; Y = tile.Y + 1} 
let inline southEast tile = {X = tile.X + 1; Y = tile.Y + 1} 

let initial =
    [|
        {FrontLeft = northWest; Front = north; FrontRight = northEast}
        {FrontLeft = southEast; Front = south; FrontRight = southWest}
        {FrontLeft = southWest; Front = west; FrontRight = northWest}
        {FrontLeft = northEast; Front = east; FrontRight = southEast}
    |]

let inline cycle (dirs : Direction array) =
    [| dirs.[1]; dirs.[2]; dirs.[3]; dirs.[0] |]
