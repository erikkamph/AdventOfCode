namespace AdventOfCode

open System.IO
open System
open System.Threading

module Main =
    type SequenceWorker(input: float array) =
        member val NumberSequence = input with get, set
        member val NumberDifferences = [|input|] with get, set
        member val AllZeroes = false with get, set
        member val LastNumber = 0.0 with get, set
        member val FirstNumber = 0.0 with get, set
        member this.Calulator() =
            while not(this.AllZeroes) do
                let next =
                    this.NumberSequence
                    |> Array.copy
                    |> Array.mapi (
                        fun idx item ->
                            if idx + 1 < this.NumberSequence.Length then
                                this.NumberSequence[idx + 1] - item 
                            else 
                                Int32.MinValue
                    )
                    |> Array.filter (fun item -> item > Int32.MinValue)

                this.NumberDifferences <- Array.insertAt 0 next this.NumberDifferences
                this.NumberSequence <- next
                this.AllZeroes <- next |> Array.filter (fun t -> t = 0) |> fun t -> t.Length = this.NumberSequence.Length
        
        member this.LookForward() =
            for idx in 1..this.NumberDifferences.Length - 1 do
                let prev = this.NumberDifferences[idx][this.NumberDifferences[idx].Length - 1]
                let curr = if this.NumberDifferences[idx - 1].Length = 0 then 0.0 else this.NumberDifferences[idx - 1][this.NumberDifferences[idx - 1].Length - 1]
                this.NumberDifferences[idx - 1] <- Array.append this.NumberDifferences[idx - 1] [|curr|]
                this.NumberDifferences[idx] <- Array.append this.NumberDifferences[idx] [|prev + curr|]
            this.LastNumber <- this.NumberDifferences[this.NumberDifferences.Length - 1][this.NumberDifferences[this.NumberDifferences.Length - 1].Length - 1]

        member this.LookBackward() =
            for idx in 1..this.NumberDifferences.Length - 1 do
                let curr = this.NumberDifferences[idx][0]
                let prev = if this.NumberDifferences[idx - 1].Length = 0 then 0.0 else this.NumberDifferences[idx - 1][0]
                this.NumberDifferences[idx - 1] <- Array.insertAt 0 curr this.NumberDifferences[idx - 1]
                this.NumberDifferences[idx] <- Array.insertAt 0 (curr - prev) this.NumberDifferences[idx]
            this.FirstNumber <- this.NumberDifferences[this.NumberDifferences.Length - 1][0]

    [<EntryPoint>]
    let main argv =
        async {
            let file = @"input.txt"
            let contents = File.ReadAllLines(file)

            printfn "Starting %d threads for calculating differences for all numbers in the series" contents.Length
            let mutable threads = []
            for row in contents do
                let input = row.Split() |> Array.map (fun t -> t |> Double.Parse)
                let sequencer = new SequenceWorker(input)
                let thread = new Thread(new ThreadStart(sequencer.Calulator))
                threads <- List.append threads [(input, sequencer, thread)]
                thread.Start()
                thread.Join()
            
            printfn "Starting %d threads for forward and backward extrapolation" (contents.Length * 2)
            let mutable new_threads = []
            for (input, sequencer, thread) in threads do
                thread.Join()
                let t2 = new Thread(new ThreadStart(sequencer.LookBackward))
                let t3 = new Thread(new ThreadStart(sequencer.LookForward))
                new_threads <- List.append new_threads [(input, sequencer, t2, t3)]
                t2.Start()
                t3.Start()
            
            let mutable backward = 0.0
            let mutable forward = 0.0
            for (input, sequencer, t1, t2) in new_threads do
                t1.Join()
                t2.Join()
                forward <- forward + sequencer.LastNumber
                backward <- backward + sequencer.FirstNumber

            printfn "Sum of all predicted numbers for forward extrapolation in part 1: %.0f" forward
            printfn "Sum of all predicted numbers for backward extrapolation in part 2: %.0f" backward

            return 0
        }
        |> Async.RunSynchronously