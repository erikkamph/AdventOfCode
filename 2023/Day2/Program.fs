open Home.Helpers
open Home.Types
open System

type Color =
    | Unknown = -1
    | Red = 0
    | Green = 1
    | Blue = 2

type Item() =
    member val color: Color = Color.Unknown with get, set
    member val count = 0 with get, set

type Game() =
    member val Sets: Item list list = [] with get, set
    member val Name = "" with get, set
    member val Number = 0 with get, set
    member val IsPossible = true with get, set

let MakeSetsOfArray (sets: string array) =
    sets
    |> Array.map (
        fun item ->
            item.Split(", ")
            |> Array.map (
                fun item ->
                    let parts = item.Split(" ")
                    let item = new Item()
                    item.color <-
                        match parts[1] with
                        | "red" -> Color.Red
                        | "green" -> Color.Green
                        | "blue" -> Color.Blue
                        | _ -> Color.Unknown
                    item.count <- parts[0] |> Int32.Parse
                    item
            )
            |> List.ofArray
    )
    |> List.ofArray

let ParseGame (row: string) =
    let parts = row.Split(": ")
    let game = new Game()
    game.Name <- parts[0]
    game.Number <- parts[0].Split(" ")[1] |> Int32.Parse
    game.Sets <- parts[1] |> fun s -> s.Split("; ") |> MakeSetsOfArray
    game

let ItemIsImpossible (item: Item) (filter: Filter) =
    async {
        match item.color with
        | Color.Red ->  return item.count > filter.Colors.Red
        | Color.Blue -> return item.count > filter.Colors.Blue
        | Color.Green -> return item.count > filter.Colors.Green
        | Color.Unknown -> return true
        | _ -> return true
    }

let rec IsSetImpossible (set: Item list) (filter: Filter) =
    async {
        match set with
        | [] -> return true
        | [head] -> return! ItemIsImpossible head filter
        | head::tail ->
            match! ItemIsImpossible head filter with
            | true -> return true
            | false -> return! IsSetImpossible tail filter
    }

let rec TraverseSets (sets: Item list list) (filter: Filter) =
    async {
        match sets with
        | [] -> return false
        | [head] ->
            let! verdict = IsSetImpossible head filter
            return not(verdict)
        | head::tail ->
            match! IsSetImpossible head filter with
            | false -> return! TraverseSets tail filter
            | true -> return false
    }

let rec TraverseGames (games: Game list) (filter: Filter) =
    async {
        match games with
        | [] -> return games
        | [head] ->
            let! verdict = TraverseSets head.Sets filter
            head.IsPossible <- verdict
            return [head]
        | head::tail ->
            let! verdict = TraverseSets head.Sets filter
            head.IsPossible <- verdict
            let! traversed = TraverseGames tail filter
            return [head] @ traversed            
    }

[<EntryPoint>]
let Main argv =
    async {
        let! content = GetFileContents(@"input_two.txt")
        let games = content |> Array.map (fun item -> ParseGame item) |> List.ofArray
        let! filter = GetFilter()
        let! possible = TraverseGames games filter

        possible |> List.filter (fun item -> item.IsPossible = true) |> List.map (fun item -> item.Number) |> List.sum |> printfn "%d"

        return 0
    }
    |> Async.RunSynchronously