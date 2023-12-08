namespace AdventOfCode

open System.IO
open System.Threading

module Main =
    let mutable stepstakenperpath = [||]
    let mutable paths: (string * (string * string)) array = [||]
    let semaphore = new Semaphore(1, 1)

    type PathTraverser(index: int, treearr: List<string*(string*string)>, endchar: char, instructions: string) =
        member val idx = index with get
        member val start = paths[index] with get
        member val current = paths[index] with get, set
        member val treearr = treearr with get
        member val endchar = endchar with get
        member val steps_taken = 0 with get, set
        member val truthy = true with get, set
        member val instructions = instructions with get
        member val insidx = 0 with get, set
        member this.ThreadProc() =
            while this.truthy do
                let _, (l, r) = this.current
                let instruction = this.instructions[this.insidx % instructions.Length]
                this.insidx <- this.insidx + 1
                match instruction with
                | 'R' ->
                    this.steps_taken <- this.steps_taken + 1
                    this.current <- this.treearr |> List.find (fun (n, (_, _)) -> n = r)
                | 'L' ->
                    this.steps_taken <- this.steps_taken + 1
                    this.current <- this.treearr |> List.find (fun (n, (_, _)) -> n = l)
                | _ -> ()
            
                if semaphore.WaitOne(1000) then
                    paths[this.idx] <- this.current
                    stepstakenperpath[this.idx] <- this.steps_taken
                    this.truthy <- 
                        Array.map2 (
                            fun (x, (_, _)) y ->
                                (x, y)
                        ) paths stepstakenperpath
                        |> Array.filter (
                            fun (x, y) ->
                                x.EndsWith("Z") && (stepstakenperpath |> Array.filter (fun t -> t = y) |> fun t -> t.Length = stepstakenperpath.Length)
                        )
                        |> fun t -> not(t.Length = stepstakenperpath.Length)
                    semaphore.Release() |> ignore

    [<EntryPoint>]
    let main argv =
        async {
            let file = @"input.txt"
            let! content = File.ReadAllLinesAsync(file) |> Async.AwaitTask

            let instructions = content[0]
            let treearr = 
                content[2..] 
                |> Array.map (
                    fun item -> 
                        let parts = item.Split(" = ")
                        let l, r = parts[1].Replace("(","").Replace(")", "").Split(", ") |> fun p -> p[0], p[1]
                        parts[0], (l, r)
                )
                |> Array.sortBy (fun (t, _) -> t)
            
            printfn "Running PART 1"
            let mutable curr, (l, r) = treearr[0]
            for n in 0..instructions.Length * 10000 do
                let follow = instructions[n % instructions.Length]
                if not(curr.Equals("ZZZ"))then
                    match follow with
                    | 'R' ->
                        paths <- Array.append paths [|treearr |> Array.find (fun (t, (_, _)) -> t = r)|]
                        treearr 
                        |> Array.find (fun (c, _) -> c = r) 
                        |> fun (c, (m, n)) ->
                            curr <- c
                            l <- m
                            r <- n
                    | 'L' ->
                        paths <- Array.append paths [|treearr |> Array.find (fun (t, (_, _)) -> t = l)|]
                        treearr 
                        |> Array.find (fun (c, _) -> c = l) 
                        |> fun (c, (m, n)) ->
                            curr <- c
                            l <- m
                            r <- n
                    | _ -> ()
            
            match file with
            | "debug.txt" -> assert (paths.Length = 6)
            | "example.txt" -> assert (paths.Length = 2)
            | "input.txt" -> printfn "Total number of steps taken from AAA to ZZZ: %d" paths.Length 
            | _ -> ()

            printfn "Running PART 2"
            paths <- treearr |> Array.copy |> Array.filter (fun (c, (_, _)) -> c.EndsWith("A"))
            stepstakenperpath <- Seq.init paths.Length (fun i -> -1) |> Seq.toArray

            let mutable threads = []
            paths
            |> Array.iteri (
                fun idx item ->
                    let traverser = new PathTraverser(idx, (treearr |> List.ofArray), 'Z', instructions)
                    let thread = new Thread(new ThreadStart(traverser.ThreadProc))
                    threads <- List.append threads [(traverser, thread)]
                    thread.Start()
            )

            printfn "%d" threads.Length

            for n, t in (threads |> List.rev) do
                t.Join()
                printfn "%A %A %d %A %A" n.start n.current n.steps_taken stepstakenperpath paths

            return 0
        }
        |> Async.RunSynchronously