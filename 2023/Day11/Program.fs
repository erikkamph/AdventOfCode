namespace AdventOfCode

open System
open System.IO
open System.Threading

module Main =
    type Galaxy(n: int, c: char, x: int, y: int) =
        member val Character = c with get
        member val Number = n with get
        member val X = x with get
        member val Y = y with get

    type ExpandUniverse(universe: string array) =
        member val Universe = (universe |> Array.copy |> Array.map (fun line -> line |> Seq.toArray)) with get, set
        member val ExpandedLines = (universe |> Array.copy |> Array.map (fun line -> line |> Seq.toArray)) with get, set
        member val Copy = [||] with get, set        
        member this.ExpandLines() =
            let empty = String.init this.ExpandedLines[0].Length (fun _ -> ".") |> Seq.toArray
            this.ExpandedLines
            |> Array.iter (
                fun line ->
                    this.Copy <- Array.append this.Copy [|line|]
                    if line = empty then
                        this.Copy <- Array.append this.Copy [|line|]
            )
            this.ExpandedLines <- this.Copy |> Array.copy
        member this.Transpose() =
            this.Copy
            |> Array.copy
            |> Array.transpose
            |> fun transposed -> this.ExpandedLines <- transposed |> Array.copy
        member this.ExpandColumns() =
            let emptycolumn = String.init this.ExpandedLines[0].Length (fun _ -> ".") |> Seq.toArray
            this.ExpandedLines
            |> Array.iter (
                fun row ->
                    this.Copy <- Array.append this.Copy [|row|]
                    if row = emptycolumn then
                        this.Copy <- Array.append this.Copy [|row|]
            )
            this.ExpandedLines <- this.Copy |> Array.copy
        member this.Expand() =
            printfn "Before universe expanded: %d*%d (Rows*Columns)" this.ExpandedLines.Length this.ExpandedLines[0].Length
            this.ExpandLines()
            this.Transpose()
            this.Copy <- [||]
            this.ExpandColumns()
            this.Transpose()
            printfn "After universe expanded: %d*%d (Rows*Columns)" this.ExpandedLines.Length this.ExpandedLines[0].Length

    [<EntryPoint>]
    let main argv =
        async {
            let file = @"input.txt"
            let! contents = File.ReadAllLinesAsync(file) |> Async.AwaitTask

            let expander = new ExpandUniverse(contents)
            let thread = new Thread(new ThreadStart(expander.Expand))
            thread.Start()
            thread.Join()


            let mutable galaxies = [||]
            let res = expander.ExpandedLines
            res
            |> Array.iteri (
                fun y line ->
                    line |> Array.iteri (
                        fun x c ->
                            if c = '#' then
                                galaxies <- Array.append galaxies [|new Galaxy(galaxies.Length + 1, c, x, y)|]
                    )
            )

            printfn "Number of galaxies: %d" galaxies.Length

            
            galaxies
            |> Array.copy
            |> Array.map (fun item ->
                galaxies
                |> Array.copy
                |> Array.filter (fun g -> g.Number <> item.Number)
                |> Array.map (fun g -> (item, g))
            )
            |> Array.map (
                fun arr ->
                    arr
                    |> Array.map (
                        fun (f, g) ->
                            let x = abs(f.X - g.X)
                            let y = abs(f.Y - g.Y)
                            x + y - 2 + 1
                    )
                    |> Array.sort
                    |> fun t -> t[0]
            )
            |> Array.sum
            |> printfn "%d"

            return 0
        }
        |> Async.RunSynchronously