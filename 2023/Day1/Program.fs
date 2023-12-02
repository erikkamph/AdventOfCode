open System.IO
open System.Text
open System

let words = "one,two,three,four,five,six,seven,eight,nine".Split(",") |> List.ofArray
let digitmapping = Seq.init 9 (fun item -> (words[item], item + 1)) |> Map.ofSeq

let (|IsString|_|) (a: obj) =
    try
        Some(a :?> string)
    with
    | ex ->
        None

let rec Contains (items: string list) (part: string) (charindex: int) =
    match items with
    | [] -> None
    | [head] ->
        if charindex + head.Length > part.Length then
            None
        else
            let substr = part.Substring(charindex, head.Length)
            match substr = head with
            | true -> Some(digitmapping[head])
            | false -> None
    | head::tail ->
        if charindex + head.Length > part.Length then
            Contains tail part charindex
        else
            let substr = part.Substring(charindex, head.Length)
            match substr = head with
            | true -> Some(digitmapping[head])
            | false -> Contains tail part charindex

let GetAllDigits (part: string) =
    part |> Seq.mapi (
        fun index char ->
            match Char.IsAsciiDigit char with
            | true -> Some($"{char}" |> Int32.Parse)
            | false -> Contains words part index
    )
    |> Seq.filter (fun item -> item.IsSome)
    |> Seq.map (fun item -> item.Value)
    |> List.ofSeq

let AssignmentOne file =
    File.ReadAllLines(file)
    |> List.ofArray
    |> List.map (fun item -> item |> Seq.find (fun item -> Char.IsDigit(item)), item |> Seq.findBack (fun item -> Char.IsDigit(item)))
    |> List.map (fun (a, b) -> $"{a}{b}" |> Int32.Parse)
    |> List.sum
    |> printfn "Assignment 1, Sum of digits only: %d"

let AssignmentTwo file =
    File.ReadAllLines(file)
    |> Array.map (fun input -> GetAllDigits input)
    |> Array.map (fun input -> input.Head, (input |> List.rev).Head)
    |> Array.map (fun (a, b) -> $"{a}{b}" |> Int32.Parse)
    |> Array.sum
    |> printfn "Assignment 2, Sum of first and last digit of words and digit: %d"

[<EntryPoint>]
let Main argv =
    AssignmentOne @"example_one.txt"
    AssignmentTwo @"example_two.txt"
    0