open System.IO
open System.Text
open System
open AdventOfCode.Types
open AdventOfCode.Patterns

let weight = "AKQJT98765432" |> Seq.mapi (fun idx item -> (item,  14 - idx + 1)) |> Map.ofSeq

let CheckHand (h: Hand) =
    let weight = h.Weight |> Convert.ToDouble
    let mutable divider = 100.0
    
    match h with
    | FiveOfAKind _ -> h.GameKindWeight <- 7
    | FourOfAKind _ -> h.GameKindWeight <- 6
    | FullHouse _ -> h.GameKindWeight <- 5
    | ThreeOfAKind _ -> h.GameKindWeight <- 4
    | TwoPair _ -> h.GameKindWeight <- 3
    | OnePair _ -> h.GameKindWeight <- 2
    | HighCard _ -> h.GameKindWeight <- 1
    | _ -> h.GameKindWeight <- -1

    h.FinalWeight <- (weight / divider)
    h

let SetWeightBasedOnCountOfEachChar (hand: Hand) =
    hand.CountOfC <- hand.Hand |> Seq.toList |> List.countBy (fun t -> t) |> List.sortByDescending (fun (c, n) -> n)
    hand.Weight <- hand.CountOfC |> List.map (fun (c, n) -> weight[c] * n) |> List.sum
    CheckHand hand

let SortHands (hands: Hand list) = 
    let mutable sorted = [hands[0]]
    let mutable hasBeenInserted = false

    for hand in hands[1..] do
        hasBeenInserted <- false

        sorted
        |> List.iteri (
            fun idx item ->
                item.Hand |> Seq.iteri (
                    fun i c ->
                        if weight[c] > weight[hand.Hand[i]] && not(hasBeenInserted) then
                            sorted <- List.insertAt idx hand sorted
                            hasBeenInserted <- true
                        elif weight[c] < weight[hand.Hand[i]] && not(hasBeenInserted) then
                            sorted <- List.insertAt (idx + 1) hand sorted
                            hasBeenInserted <- true
                        elif item.GameKindWeight > hand.GameKindWeight && not(hasBeenInserted) then
                            sorted <- List.insertAt idx hand sorted
                            hasBeenInserted <- true
                        elif item.GameKindWeight < hand.GameKindWeight && not(hasBeenInserted) then
                            sorted <- List.insertAt (idx + 1) hand sorted
                            hasBeenInserted <- true
                        else ()
                )
        )
    sorted

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
            |> List.ofArray
        
        printf "Initial scoring by hand: "
        hands |> List.sortBy (fun t -> t.Weight) |> List.iter (fun item -> printf "(%A %d) " item.Hand item.Bid)
        printfn ""

        hands
        |> SortHands
        |> fun l ->
            printf "Sorted: "
            l |> List.iter (fun t -> printf "(%s, %d) " t.Hand t.Bid)
            printfn "" 
            l
        |> List.mapi (fun idx hand -> (idx + 1) * hand.Bid) |> List.sum |> printfn "Final sum of cards bid * rank:  %d"
        return 0
    }
    |> Async.RunSynchronously