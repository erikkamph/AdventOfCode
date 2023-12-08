namespace AdventOfCode

open System.IO
open System

module Main =
    let run (folded: (int*int) array) =
        let mutable n = 1
        for time, highscore in folded do
            let mutable possibledistances = Seq.init time (fun t -> t) |> Seq.toArray
            printfn "%A" possibledistances
            possibledistances
            |> Array.iteri (
                fun idx i ->
                    let remaining = time - i
                    let length = i * remaining
                    possibledistances[idx] <- length
            )
            let validdistances = possibledistances |> Array.filter (fun n -> n > highscore)
            n <- n * validdistances.Length
        n

    [<EntryPoint>]
    let main argv =
        async {
            let file = @"example.txt"
            let content = File.ReadAllLines(file)

            let time, distance = content |> fun t -> t[0], t[1]
            
            let p1time = time.Replace("Time: ", "").Split() |> Array.filter (fun t -> t <> "") |> Array.map (fun item -> item |> Int32.Parse)
            let p1distance = distance.Replace("Distance:", "").Split() |> Array.filter (fun t -> t <> "") |> Array.map (fun item -> item |> Int32.Parse)

            let folded = Array.map2 (fun t y -> (t, y)) p1time p1distance
            let res = run folded
            printfn "Part 1 Answer: %d" res

            let p1time = time.Replace("Time: ", "").Replace(" ", "") |> Int32.Parse
            let p1distance = distance.Replace("Distance:", "").Replace(" ", "") |> Int32.Parse
            let res = run [|(p1time, p1distance)|]
            printfn "Part 2 Answer: %d" res

            return 0
        }
        |> Async.RunSynchronously