namespace AdventOfCode

open System.IO
open System
open FSharp.Data.Adaptive

module Main =
    let rec SumOfChars (total: int) (chars: char list)  =
        async {
            match chars with
            | [] -> return total
            | [head] -> return ((((head |> Convert.ToInt32) + total) * 17) % 256)
            | head::tail -> return! SumOfChars ((((head |> Convert.ToInt32) + total) * 17) % 256) tail
        }

    let ExtractIntegers (input: string) =
        async {
            try 
                let mutable res = 0
                input
                |> Seq.filter (fun t -> Char.IsDigit(t))
                |> Seq.map (fun t -> $"{t}")
                |> String.concat ""
                |> fun t -> t
                |> Int32.Parse
                |> fun t -> res <- t
                return res
            with
            | ex ->
                printfn $"{ex.Message} {ex.StackTrace}"
                return Int32.MinValue
        }

    let Modify (box: string list) (item: string) =
        async {
            let key = item |> Seq.filter (fun t -> Char.IsAsciiLetter(t)) |> Seq.map (fun t -> $"{t}") |> String.concat ""
            let existing = box |> List.tryFindIndex (fun item -> item.StartsWith(key))
            match existing with
            | None -> 
                if item.Contains("=") then 
                    return box |> List.append [item] 
                else 
                    return box
            | Some t -> 
                if item.Contains("-") then
                    return box |> List.removeAt t 
                elif item.Contains("=") then 
                    return box |> Array.ofList |> fun m -> m[t] <- item; m |> List.ofArray 
                else 
                    return box
        }
    
    [<EntryPoint>]
    let main argv =
        async {
            let file = $"{argv[0]}.txt"
            let! contents = File.ReadAllLinesAsync(file) |> Async.AwaitTask
            let initseq = contents |> String.concat "," |> fun s -> s.Split(",")
            
            let mutable items = []
            for item in initseq do
                let! sum = SumOfChars 0 (item |> Seq.toList)
                items <- items @ [sum]

            printfn "Part 1 Array: %A" items
            printfn "Part 1 Sum: %d" (items |> List.sum)

            let mutable boxes = HashMap.empty<int, string list>
            for item in initseq do
                let key = item |> Seq.toList |> List.filter (fun t -> Char.IsAsciiLetter(t))
                let! pos = SumOfChars 0 key
                let box = boxes.ToKeyList() |> List.contains pos
                match box with
                | false -> if item.Contains("=") then boxes <- boxes.Add(pos, [item])
                | true ->
                    let! new_box = Modify boxes[pos] item
                    boxes <- boxes.Remove(pos).Add(pos, new_box)
            
            printfn "Part 2 mapping: %A" boxes 
            let mutable sum = 0
            for key, box in boxes do
                for item in box |> List.rev do
                    let pos = box |> List.rev |> List.findIndex (fun i -> i = item)
                    let! length = ExtractIntegers item
                    sum <- sum + ((key + 1) * (pos + 1) * length)

            printfn "Part 2 Sun: %d" sum

            return 0
        }
        |> Async.RunSynchronously