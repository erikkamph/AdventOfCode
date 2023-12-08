open AdventOfCode.Helpers
open AdventOfCode.Types
open System

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
        let! content = GetFileContents(@"example.txt")
        let games = content |> Array.map (fun item -> ParseGame item) |> List.ofArray
        let! filter = GetFilter()
        let! possible = TraverseGames games filter

        possible |> List.filter (fun item -> item.IsPossible) |> List.map (fun item -> item.Number) |> List.sum |> printfn "Sum of possible games: %d"
        games |> List.map (
            fun item ->
                let mutable maxred = 0
                let mutable maxgreen = 0
                let mutable maxblue = 0
                item.Sets
                |> List.concat
                |> List.iter (
                    fun item ->
                        match item.color with
                        | Color.Red -> maxred <- if maxred < item.count then item.count else maxred
                        | Color.Green -> maxgreen <- if maxgreen < item.count then item.count else maxgreen
                        | Color.Blue -> maxblue <- if maxblue < item.count then item.count else maxblue
                        | _ -> printfn "Haha"
                )
                let res = Math.BigMul(maxred, maxgreen)
                let mulres = Math.BigMul(res |> Convert.ToInt32, maxblue)
                mulres
        )
        |> List.sum
        |> printfn "Sum of power of all games: %d"

        return 0
    }
    |> Async.RunSynchronously