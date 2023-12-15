namespace AdventOfCode

open System
open System.IO
open System.Threading

module main =
    type Position(c: char, x: int, y: int) =
        member val StartChar = ' ' with get, set
        member val Column = x with get, set
        member val Row = y with get, set
    

    
    [<EntryPoint>]
    let main argv =
        async {
            let file = @"example.txt"
            let contents = File.ReadAllLines(file)

            contents
            |> Array.mapi (
                fun row line ->
                    line 
                    |> Seq.mapi (fun col char -> (col, char)) 
                    |> Seq.filter (fun (n, _) -> n <> Int32.MinValue)
                    |> Seq.map (
                        fun (col, c) ->
                            new Position(c, col, row)
                    )
                    |> Seq.toArray
            )
            |> Array.concat
            |> Array.map (fun item -> (item.Column, item.Row))
            |> printfn "%A"

            return 0
        }
        |> Async.RunSynchronously