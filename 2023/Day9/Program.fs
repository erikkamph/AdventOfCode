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

        member this.Run() =
            this.Calulator()
            this.LookBackward()
            this.LookForward()

    [<EntryPoint>]
    let main argv =
        async {
            let file = @"input.txt"
            let contents = File.ReadAllLines(file)

            let mutable forward, backward = (0.0, 0.0)
            contents
            |> Array.map (
                fun row ->
                    let input = row.Split() |> Array.map(fun item -> item |> Double.Parse)
                    let sequencer = new SequenceWorker(input)
                    let thread = new Thread(new ThreadStart(sequencer.Run))
                    thread.Start()
                    (sequencer, thread)
            )
            |> Array.iter (
                fun (sequencer, thread) ->
                    forward <- forward + sequencer.LastNumber
                    backward <- backward + sequencer.FirstNumber
            )
            
            printfn "Forward extrapolated values: %.0f" forward
            printfn "Backward extrapolated values: %.0f" backward

            return 0
        }
        |> Async.RunSynchronously