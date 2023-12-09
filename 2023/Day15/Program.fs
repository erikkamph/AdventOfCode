namespace AdventOfCode

open System.IO
open System

module Main =
    [<EntryPoint>]
    let main argv =
        async {
            let file = $"{argv[0]}.txt"
            let! contents = File.ReadAllLinesAsync(file) |> Async.AwaitTask
            let initseq = contents |> String.concat "," |> fun s -> s.Split(",")
            
            initseq
            |> Array.map (
                fun s ->
                    let mutable start = 0
                    s
                    |> Seq.map (
                        fun c -> c |> Convert.ToInt32
                    )
                    |> Seq.iter (
                        fun n ->
                            start <- start + n
                            start <- start * 17
                            start <- start % 256
                    )
                    start
            )
            |> fun s ->
                printfn "Part 1 Array: %A" s
                printfn "Part 1 Sum: %d" (s |> Array.sum)

            let mutable boxes: string array array =
                Array.init 256 (
                    fun _ ->
                        [||]
                )
            let mutable box = 0
            for item in initseq do
                box <- 0
                item |> Seq.filter (fun t -> t <> '=' && t <> '-' && not(Char.IsDigit(t))) |> Seq.map (fun n -> n |> Convert.ToInt32) |> Seq.iter (fun n -> box <- box + n; box <- box * 17; box <- box % 256)
                if item.Contains("-") then
                    let itemtoremove = boxes[box] |> Array.tryFind (fun n -> n.StartsWith(item.Replace("-", "=")))
                    match itemtoremove with
                    | Some t -> 
                        boxes[box] <- boxes[box] |> Array.filter (fun n -> n <> t)
                    | None -> ()
                elif item.Contains("=") then
                    let exists = boxes[box] |> Array.tryFindIndex (fun n -> n.StartsWith(item.Split("=")[0]))
                    match exists with
                    | Some idx -> boxes[box][idx] <- item
                    | None -> boxes[box] <- Array.append boxes[box] [|item|]
            
            printfn "%A" boxes
            boxes
            |> Array.mapi (
                fun box items ->
                    items
                    |> Array.mapi (
                        fun slot item ->
                            (box + 1) * (slot + 1) * (item |> Seq.filter (fun t -> Char.IsDigit(t)) |> Seq.map (fun t -> $"{t}") |> String.concat "" |> fun t -> t |> Int32.Parse)
                    )
            )
            |> fun s ->
                printfn "Part 2 Mapping: %A" boxes
                printfn "Part 2 Focusing Power: %A" s
                s |> Array.concat |> Array.sum |> printfn "Part 2 Total Focusing Power: %d"

            return 0
        }
        |> Async.RunSynchronously