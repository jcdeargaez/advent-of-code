#r "nuget: Fparsec"

open System.Collections.Generic
open System.IO

[<Literal>]
let Root = "root"

[<Literal>]
let Humn = "humn"

type Operator =
    | Sum
    | Substract
    | Multiply
    | Divide

type Job =
    | Number of int64
    | Operation of string * Operator * string

type Monkey = {
    Name : string
    Job : Job
}

module Job =
    let operation left right operator =
        let op =
            match operator with
            | Sum       -> (+)
            | Substract -> (-)
            | Multiply  -> ( * )
            | Divide    -> (/)
        op left right

module Monkey =
    let isolate targetName monkey =
        match monkey.Job with
        | Operation (leftName, Sum, rightName) when leftName = targetName        -> { Name = leftName; Job = Operation (monkey.Name, Substract, rightName) }
        | Operation (leftName, Sum, rightName) when rightName = targetName       -> { Name = rightName; Job = Operation (monkey.Name, Substract, leftName) }
        | Operation (leftName, Substract, rightName) when leftName = targetName  -> { Name = leftName; Job = Operation (monkey.Name, Sum, rightName) }
        | Operation (leftName, Substract, rightName) when rightName = targetName -> { Name = rightName; Job = Operation (leftName, Substract, monkey.Name) }
        | Operation (leftName, Multiply, rightName) when leftName = targetName   -> { Name = leftName; Job = Operation (monkey.Name, Divide, rightName) }
        | Operation (leftName, Multiply, rightName) when rightName = targetName  -> { Name = rightName; Job = Operation (monkey.Name, Divide, leftName) }
        | Operation (leftName, Divide, rightName) when leftName = targetName     -> { Name = leftName; Job = Operation (monkey.Name, Multiply, rightName) }
        | Operation (leftName, Divide, rightName) when rightName = targetName    -> { Name = rightName; Job = Operation (leftName, Divide, monkey.Name) }
        | Number _ -> monkey
        | _ -> failwith "Unexpected operation"

module Operations =
    let traverse rootName (monkeys : IDictionary<string, Monkey>) =
        let results = Dictionary monkeys.Count
        let rec visit =
            function
            | [] -> results
            | name :: names ->
                let nextNames =
                    let monkey = monkeys.[name]
                    match monkey.Job with
                    | Number n ->
                        results.Add (monkey.Name, n)
                        List.empty
                    | Operation (leftName, operator, rightName) ->
                        match results.ContainsKey leftName, results.ContainsKey rightName with
                        | false, false -> [leftName; rightName; name]
                        | false, true -> [leftName; name]
                        | true, false -> [rightName; name]
                        | true, true ->
                            let result =
                                operator
                                |> Job.operation results.[leftName] results.[rightName]
                            results.Add (monkey.Name, result)
                            List.empty
                visit (nextNames @ names)
        visit [rootName]

    let findYellingOrPartnerMonkey (monkeys : IDictionary<string, Monkey>) name =
        monkeys.Values
        |> Seq.tryFind (fun m ->
            // Prefer monkey that yells a number, except for the human
            m.Name = name && m.Name <> Humn &&
            match m.Job with
            | Number _  -> true
            | _ -> false)
        |> Option.defaultWith (fun () ->
            // If the monkey doesn't yell a number, find the operation in which he participates
            monkeys.Values
            |> Seq.find (fun m ->
                match m.Job with
                | Operation (leftName, _, rightName) when leftName = name || rightName = name -> true
                | _ -> false))

    let newMonkeys monkeys (results : Dictionary<string, int64>) =
        let visited = HashSet ()
        let rec visit reformulatedMonkeysSoFar =
            function
            | [] -> reformulatedMonkeysSoFar
            | (monkey, targetName) :: items ->
                    let isolatedMonkey =
                        if visited.Contains monkey.Name then
                            // Yelling a number is needed
                            { Name = targetName; Job = Number results.[targetName] }
                        elif monkey.Name = "root" then
                            // Fixing the root is needed
                            let partnerName =
                                match monkey.Job with
                                | Operation (leftName, _, rightName) when leftName = targetName -> rightName
                                | Operation (leftName, _, rightName) when rightName = targetName -> leftName
                                | _ -> failwith "root monkey job should be an operation"
                            { Name = targetName; Job = Number results.[partnerName] }
                        else
                            monkey
                            |> Monkey.isolate targetName
                    
                    let nextItems =
                        match isolatedMonkey.Job with
                        | Operation (leftName, mo, rightName) ->
                            let leftMonkey =
                                leftName
                                |> findYellingOrPartnerMonkey monkeys
                            let rightMonkey =
                                rightName
                                |> findYellingOrPartnerMonkey monkeys
                            [leftMonkey, leftName; rightMonkey, rightName]
                        | Number n -> List.empty
                    
                    visited.Add monkey.Name |> ignore
                    visit (isolatedMonkey :: reformulatedMonkeysSoFar) (nextItems @ items)
        
        let human =
            Humn
            |> findYellingOrPartnerMonkey monkeys
        visit List.empty [human, Humn]

module Parser =
    open FParsec

    let parse input =
        let pmathOperation = choice [
            charReturn '+' Sum
            charReturn '-' Substract
            charReturn '*' Multiply
            charReturn '/' Divide
        ]
        let pname = many1Satisfy isLetter
        let poperation = tuple3 (pname .>> skipChar ' ') pmathOperation (skipChar ' ' >>. pname) |>> Operation
        let pnumber = pint64 |>> Number
        let pjob = pnumber <|> poperation
        let pmonkey = tuple2 (pname .>> skipString ": ") pjob |>> fun (name, job) -> { Name = name; Job = job }
        let pmonkeyLine = pmonkey .>> skipNewline
        let pmonkeyLinesEof = many1 pmonkeyLine .>> eof
        match input |> run pmonkeyLinesEof with
        | Success (monkeys, _, _) -> monkeys
        | Failure (errorMsg, _, _) -> failwith errorMsg

let monkeys =
    File.ReadAllText "input.txt"
    |> Parser.parse
    |> Seq.map (fun monkey -> monkey.Name, monkey)
    |> dict

let results =
    monkeys
    |> Operations.traverse Root

results.[Root]
|> printfn "Part 1: %i"

let monkeys' =
    results
    |> Operations.newMonkeys monkeys
    |> Seq.map (fun m -> m.Name, m)
    |> dict

let results' =
    monkeys'
    |> Operations.traverse Humn

results'.[Humn]
|> printfn "Part 2: %i"
