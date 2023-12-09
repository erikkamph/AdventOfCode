namespace AdventOfCode

open System
open System.IO
open System.Threading

module Main =
    [<EntryPoint>]
    let main argv =
        async {
            let file = @"input.txt"
            let contents = File.ReadAllLines(file)
            let seeds = contents[0].Replace("seeds: ", "").Split() |> Array.map (fun item -> item |> Double.Parse)
            let mutable ranges = []
            let mutable m = []
            for line in contents[2..] do
                if line.Contains("-to-") then
                    m <- []
                if not(String.IsNullOrWhiteSpace(line)) && not(line.Contains("-to-")) then
                    m <- List.append m (line.Split() |> Array.map (fun item -> item |> Double.Parse) |> List.ofArray)
                if String.IsNullOrWhiteSpace(line) then
                    ranges <- ranges @ [m]

            let mutable min_loc = Double.MaxValue

            for idx in 0..2..seeds.Length - 1 do
                let mutable current = seeds[idx]
                let mutable i = current - 1.0
                while i < seeds[idx] + seeds[idx + 1] do
                    for m in ranges do
                        for n in 0..3..m.Length - 1 do
                            let dest, src, len = (m[n], m[n + 1], m[n + 2])
                            if src <= current && (current - src) < dest then
                                if i = -1.0 then
                                    i <- src + len
                                current <- current - src + dest
                                done
                    if current < min_loc then
                        min_loc <- current

            printfn "%f" min_loc
            
            return 0
        }
        |> Async.RunSynchronously