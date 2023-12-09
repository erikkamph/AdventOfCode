namespace AdventOfCode

open System.IO
open System
open AdventOfCode.RockTypes

module main =
    [<EntryPoint>]
    let main argv =
        async {
            let file = if Array.contains "example" argv then @"example.txt" elif Array.contains "debug" argv then @"debug.txt" else @"input.txt"
            let! contents = File.ReadAllLinesAsync(file) |> Async.AwaitTask

            let rows, columns = contents.Length, contents[0].Length

            let stonemapping =
                Array2D.init rows columns (
                    fun y x ->
                        if contents[y][x] = '#' then
                            Squared(new Square(x, y))
                        elif contents[y][x] = 'O' then
                            Round(new Circular(x, y))
                        else
                            EmptySpace
                )

            let n = stonemapping |> Array2D.copy
            for i in 0..n.GetLength(1) - 1 do
                let mutable stones = Array.init (n.GetLength(1)) (fun x -> n.GetValue(x, i) :?> Stone)
                
                stones
                |> Array.copy
                |> Array.rev
                |> fun stonelist ->
                    let mutable hasallmoved = false
                    while not(hasallmoved) do
                        let mutable hasnotmoved = true
                        for stone in 0..stonelist.Length - 1 do
                            if stone + 1 <= stonelist.Length - 1 then
                                match stonelist[stone], stonelist[stone + 1] with
                                | Round _, EmptySpace -> 
                                    let tmp = stonelist[stone]
                                    stonelist[stone] <- stonelist[stone + 1]
                                    stonelist[stone + 1] <- tmp
                                    hasnotmoved <- false
                                | _, _ -> ()
                        hasallmoved <- hasnotmoved
                    stonelist |> Array.rev |> Array.iteri (fun idx item -> n.SetValue(item, idx, i))
            
            let mutable sum = 0
            n
            |> Array2D.iteri (
                fun idx idy item ->
                    match item with
                    | Round t ->
                        sum <- sum + (n.GetLength(1) - idx)
                        printf $"{t.Symbol}"
                    | Squared t -> printf $"{t.Symbol}"
                    | EmptySpace -> printf "."
                    if idy >= n.GetLength(1) - 1 then
                        printfn " %d" (n.GetLength(1) - idx)
            )
            printfn "Part 1 Total: %d" sum

            let mutable mapping = Array.init (stonemapping.GetLength(0)) (fun row -> Array.init (stonemapping.GetLength(1)) (fun col -> stonemapping.GetValue(row, col) :?> Stone))
            let rotations = 1000000000
            for x in 0..rotations do
                let copy = mapping |> Array.copy

                for col in 0..copy[0].Length - 1 do
                    let stones = Array.init copy.Length (fun row -> copy[row][col])

                    stones
                    |> Array.copy
                    |> Array.rev
                    |> fun stonelist ->
                        let mutable hasallmoved = false
                        while not(hasallmoved) do
                            let mutable hasnotmoved = true
                            for stone in 0..stonelist.Length - 1 do
                                if stone + 1 <= stonelist.Length - 1 then
                                    match stonelist[stone], stonelist[stone + 1] with
                                    | Round _, EmptySpace -> 
                                        let tmp = stonelist[stone]
                                        stonelist[stone] <- stonelist[stone + 1]
                                        stonelist[stone + 1] <- tmp
                                        hasnotmoved <- false
                                    | _, _ -> ()
                            hasallmoved <- hasnotmoved
                        stonelist |> Array.rev |> Array.iteri (fun row item -> copy[row][col] <- item)
                
                mapping <- copy |> Array.copy |> Array.map (fun arr -> arr |> Array.rev) |> Array.transpose
                if x % 10000 = 0 then
                    printfn "%.3f%%" (((x |> Convert.ToDouble) / (rotations |> Convert.ToDouble)) * 100.0)
            
            let mutable sum = 0
            mapping
            |> Array.iteri (
                fun row stones ->
                    stones
                    |> Array.iteri (
                        fun col stone ->
                            match stone, col with
                            | Round _, x when x < 3 && x >= stones.Length - 1 - 3 ->
                                sum <- sum + (mapping.Length - row)
                            | _, _ -> ()
                    )
            )
            printfn "%d" sum

            return 0
        }
        |> Async.RunSynchronously