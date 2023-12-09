open System.IO
open System.Text
open System
open AdventOfCode.Types
open AdventOfCode.Patterns

let weight = "23456789TJQKA" |> Seq.mapi (fun idx item -> (item,  idx + 1)) |> Map.ofSeq

let CheckHand (h: Hand) =
    match h with
    | FiveOfAKind _ -> h.GameKindWeight <- 7
    | FourOfAKind _ -> h.GameKindWeight <- 6
    | FullHouse _ -> h.GameKindWeight <- 5
    | ThreeOfAKind _ -> h.GameKindWeight <- 4
    | TwoPair _ -> h.GameKindWeight <- 3
    | OnePair _ -> h.GameKindWeight <- 2
    | HighCard _ -> h.GameKindWeight <- 1
    | _ -> h.GameKindWeight <- -1

    h

let SetWeightBasedOnCountOfEachChar (hand: Hand) =
    hand.CountOfC <- hand.Hand |> Seq.toList |> List.countBy (fun t -> t) |> List.sortByDescending (fun (c, n) -> n)
    hand.Weight <- hand.CountOfC |> List.map (fun (c, n) -> weight[c] * n) |> List.sum
    CheckHand hand

let SortHand (hands: Hand array) =
    let mutable sorted = [||]
    for key in weight.Keys do
        hands
        |> Array.copy
        |> Array.filter (fun item -> item.Hand[0] = key)
        |> Array.sortBy (fun item -> weight[item.Hand[0]], weight[item.Hand[1]], weight[item.Hand[2]], weight[item.Hand[3]], weight[item.Hand[4]])
        |> fun t -> sorted <- Array.insertAt 0 t sorted
    sorted |> Array.concat


[<EntryPoint>]
let main argv =
    async {
        let command_line_conf =
            argv 
            |> Array.map(fun t -> t.Split("=")[0], bool.Parse(t.Split("=")[1])) 
            |> Map.ofArray
        
        let example = 
            command_line_conf.TryFind("example")
            |> fun item ->
                match item with
                | Some n -> n
                | None -> false
        
        let debug = 
            command_line_conf.TryFind("debug")
            |> fun item ->
                match item with
                | Some n -> n
                | None -> false
        
        let file = if example then @"example.txt" elif debug then @"debug.txt" else @"input.txt"
        let contents = File.ReadAllLines(file, Encoding.UTF8)
        let hands =
            contents
            |> Array.map (
                fun line ->
                    let parts = line.Split(" ")
                    let hand = new Hand()
                    hand.Bid <- parts[1] |> Int32.Parse
                    hand.Hand <- parts[0]
                    hand |> SetWeightBasedOnCountOfEachChar
            )
            
        let mutable sortedbytype = Array.init 7 (fun _ -> [||])
        sortedbytype
        |> Array.iteri (
            fun i _ ->
                sortedbytype[i] <- hands |> Array.copy |> Array.filter (fun hand -> hand.GameKindWeight = i + 1)
                printfn "%d %d" sortedbytype[i].Length (i + 1)
        )

        let mutable sortedbytypebyscore = Array.init 7 (fun _ -> [||])
        sortedbytype
        |> Array.iteri (
            fun idx cardhands ->
                sortedbytypebyscore[idx] <- cardhands |> SortHand
                //sortedbytypebyscore[idx] |> Array.iter (fun t -> printf $"{t.Weight},{t.Hand},{t.Bid} ")
                //printfn ""
        )

        let score = sortedbytypebyscore |> Array.copy |> Array.concat |> Array.mapi (fun idx item -> (idx + 1) * item.Bid) |> Array.sum
        if file = @"debug.txt" then
            printfn $"Score: {score}"
            assert (score = 816)
        elif file = @"example.txt" then
            printfn $"Score: {score}"
            assert (score = 6440)
        else
            printfn $"Score: {score}"
        

        return 0
    }
    |> Async.RunSynchronously