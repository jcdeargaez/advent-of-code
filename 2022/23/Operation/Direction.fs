module Direction

open Domain

let initial = [| North; South; West; East |]

let cycle (dirs : Direction array) =
    [| dirs.[1]; dirs.[2]; dirs.[3]; dirs.[0] |]
