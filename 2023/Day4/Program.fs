open System.IO
open System.Text
open System
open System.Text.Json

type Lottery() =
    member val GameName = "" with get, set
    member val GameNumber = 0 with get, set
    member val Numbers = List.empty<int> with get, set
    member val Winning = List.empty<int> with get, set
    member val Points = 0 with get, set
    member val NumbersWinningCount = 0 with get, set
    member this.Copy() =
        let lottery = new Lottery()
        lottery.GameName <- this.GameName
        lottery.GameNumber <- this.GameNumber
        lottery.Numbers <- this.Numbers
        lottery.Points <- this.Points
        lottery.Winning <- this.Winning
        lottery

let TransleteToLottery (row: string) =
    let parts = row.Split(": ")
    let lottery = new Lottery()
    lottery.GameName <- parts[0]
    lottery.GameNumber <- parts[0] |> Seq.findIndexBack (fun p -> p = ' ') |> fun res -> parts[0].Substring(res) |> Int32.Parse
    let gameparts = parts[1].Split(" | ")
    lottery.Winning <- gameparts[0].Split(" ") |> List.ofArray |> List.filter (fun p -> p <> "") |> List.map (fun item -> item |> Int32.Parse)
    lottery.Numbers <- gameparts[1].Split(" ") |> List.ofArray |> List.filter (fun p -> p <> "") |> List.map (fun item -> item |> Int32.Parse)
    lottery

let AssignmentFour (file: string) (mode: Encoding option) =
    match mode with
    | Some enc -> File.ReadAllLines(file, enc)
    | None -> File.ReadAllLines(file, Encoding.UTF8)

[<EntryPoint>]
let Main argv =
    let lotterygames = 
        AssignmentFour @"input_four.txt" None
        |> Array.map (fun item -> TransleteToLottery item)
        |> Array.map (
            fun game ->
                let mutable count = 0
                game.Numbers |> List.iter (fun item -> if List.contains item game.Winning then count <- count + 1 else ())
                game.NumbersWinningCount <- count
                count <- count - 1
                game.Points <- Math.Pow(2, count) |> Convert.ToInt32
                game
        )

    lotterygames
    |> fun p ->
        printfn "Points per game %A" (p |> Array.map (fun item -> item.Points))
        printfn "Total points %d" (p |> Array.map (fun item -> item.Points) |> Array.sum)
        printfn "Original number of games %d" p.Length
    
    let data = lotterygames |> JsonSerializer.Serialize
    File.WriteAllText(@"games.json", data)

    0