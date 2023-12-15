namespace AdventOfCode

open AdventOfCode.Interfaces
open System
open System.Threading
open System.IO

module main =
    let rec Faculty (number: int) =
        async {
            match number with
            | 0 -> return 1
            | 1 -> return 1
            | n -> 
                let! m = Faculty (n - 1)
                return n * m
        }

    type Mutator(initial: string) =
        member val Initial = initial.Split()[0] with get
        member val BrokenSprings = (initial.Split()[1]).Split(",") |> Array.map (fun item -> item |> Int32.Parse) with get
        member val Arrangements = [] with get, set
        member val RowLength = (initial.Split()[0]).Length with get
        interface Mutations with
            override this.IsInCombination(line, mutations) =
                async {
                    return List.contains line this.Arrangements
                }
            override this.Mutate() =
                async {
                    let broken =
                        this.BrokenSprings
                        |> Array.copy
                        |> Array.map (
                            fun s ->
                                String.init s (fun _ -> "#")
                        )
                    let mutable line = String.init this.RowLength (fun _ -> ".")
                    
                    let pc = this.Initial |> Seq.toArray |> Array.mapi (fun i c -> (i, c))
                    for (p, c) in pc do
                        if c = '#' then
                            line <- 
                                line
                                |> Seq.toArray 
                                |> Array.mapi (fun n x -> if n = p then $"{c}" else $"{x}")
                                |> String.concat ""

                    return line
                }
            override this.CalculateMutations() =
                async {
                    let n = this.RowLength
                    let r = (this.BrokenSprings |> Array.sum)
                    
                    let! a = Faculty n 
                    let! b = Faculty (r + 1)
                    let! c = Faculty (n - r)
                    printfn "%d %d %d %d" a b c (a / (b * c))

                    let combinations = a / (b * c)

                    return combinations
                }
        interface FunctionsRunner with
            override this.Run() =
                task {
                    let! n = (this :> Mutations).CalculateMutations()
                    return [|n|]
                }
    
    [<EntryPoint>]
    let main argv =
        async {
            let file = if Array.contains "example" argv then @"example.txt" elif Array.contains "debug" argv then @"debug.txt" else @"input.txt"
            let! contents = File.ReadAllLinesAsync(file) |> Async.AwaitTask

            let mutators =
                contents
                |> Array.map (
                    fun line ->
                        new Mutator(line)
                )
            
            let mutator = mutators[0]
            let runner = mutator :> FunctionsRunner
            let! res = runner.Run() |> Async.AwaitTask
            printfn "%A" res
            
            return 0
        }
        |> Async.RunSynchronously