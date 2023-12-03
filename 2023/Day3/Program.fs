open System
open System.IO
open System.Text

let expected_symbols = "467,35,633,617,592,755,664,598".Split(",") |> List.ofArray
let expected_sum = 4361

type Symbol() =
    member val Symbol = ' ' with get, set
    member val Row = 0 with get, set
    member val Column = 0 with get, set

type ValueNumber() =
    member val Number = "" with get, set
    member val ActualValue = -1 with get, set
    member val StartIndex = -1 with get, set
    member val EndIndex = - 1 with get, set
    member val Row = -1 with get, set
    member val DistancesToSymbol = List.empty with get, set


let inline (>=<) a (b,c) = a >= b && a <= c

let GetContents file =
    File.ReadAllLines(file, Encoding.UTF8)

let MainFun (fname: string) (test: bool) =
    let content = GetContents fname |> Array.toList 
    let numbers =
        content
        |> List.mapi (
            fun idx line ->
                let mutable l = []
                let mutable v = new ValueNumber()
                v.Row <- idx

                line
                |> Seq.iteri (
                    fun col c ->
                        if Char.IsDigit c then
                            v.Number <- v.Number + $"{c}"
                        
                        if v.StartIndex = -1 then
                            v.StartIndex <- col
                        v.EndIndex <- col
                        
                        if c = '.' then
                            if v.Number <> "" then
                                v.ActualValue <- v.Number |> Int32.Parse
                            l <- List.append l [v]
                            v <- new ValueNumber()
                )

                l
        )
        |> List.concat
    
    let symbols =
        content
        |> List.mapi (
            fun row line ->
                line
                |> Seq.mapi (
                    fun col c ->
                        let tmp = new Symbol()
                        tmp.Symbol <- c
                        tmp.Row <- row
                        tmp.Column <- col
                        tmp
                )
                |> Seq.toList
        )
        |> List.concat
        |> List.filter (fun t -> t.Symbol <> '.' && not(Char.IsDigit t.Symbol))
    
    symbols |> List.map (fun t -> t.Symbol) |> printfn "%A"
    
    symbols
    |> List.iter (
        fun symbol ->
            for idx in 0..numbers.Length - 1 do
                let number = numbers[idx]
                if number.Row - symbol.Row >=< (-1, 1) then
                    if number.StartIndex - symbol.Column >=< (-1, 1) || number.EndIndex - symbol.Column >=< (-1, 1) then
                        printfn $"{number.ActualValue}"
                elif symbol.Row - number.Row >=< (-1, 1) then
                    printfn $"{number.ActualValue}"
    )

    numbers 
    |> List.filter (fun p -> p.ActualValue > 0)
    |> List.filter (fun t -> t.DistancesToSymbol.Length > 0) //t.DistancesToSymbol |> List.filter (fun y -> y <= 1) |> fun c -> c.Length > 0)
    //|> List.map (fun t -> $"{t.ActualValue} - Distances (row, start, end) = {t.DistancesToSymbol}")
    //|> printfn "%A"
    
    
[<EntryPoint>]
let main argv =
    let IsTest =
        match argv.Length with
        | 0 -> false
        | _ -> argv[0] |> bool.Parse
    
    match IsTest with
    | true -> MainFun @"example.txt" true |> ignore
    | false -> MainFun @"input.txt" false |> ignore
    0