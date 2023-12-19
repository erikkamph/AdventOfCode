namespace AdventOfCode

open System.IO
open System.Threading

module Main =
    let inline (>=<) (a) (b, c) = a >= b && a <= c

    type Direction =
        | None = -1
        | North = 0
        | East = 1
        | South = 2
        | West = 3

    type Symbol(symbol: char) =
        member val Symbol = symbol with get, set
        member val Beams = 0 with get, set

    type Splitter(symbol: char, x: int, y: int) =
        inherit Symbol(symbol)
        member val Row = y with get
        member val Column = x with get
        member val North = (y - 1, x)
        member val South = (y + 1, x)
        member val West = (y, x - 1)
        member val East = (y, x + 1)
        member this.SplitTo(direction: Direction, x: int, y: int) =
            match direction with
            | Direction.East when this.Symbol = '-' -> Direction.East, Direction.None
            | Direction.West when this.Symbol = '-' -> Direction.None, Direction.West
            | Direction.North when this.Symbol = '-' -> Direction.West, Direction.East
            | Direction.South when this.Symbol = '-' -> Direction.West, Direction.East
            | Direction.East when this.Symbol = '|' -> Direction.North, Direction.South
            | Direction.West when this.Symbol = '|' -> Direction.North, Direction.South
            | Direction.North when this.Symbol = '|' -> Direction.North, Direction.None
            | Direction.South when this.Symbol = '|' -> Direction.None, Direction.South
            | _ -> Direction.None, Direction.None

    type Mirror(symbol: char, x: int, y: int) =
        inherit Symbol(symbol)
        member val Row = y with get
        member val Column = y with get
        member val North = (y - 1, x)
        member val South = (y + 1, x)
        member val West = (y, x - 1)
        member val East = (y, x + 1)
        member this.ChangeDirection(direction: Direction, x: int, y: int) =
            match direction with
            | Direction.North when this.Symbol = '\\' -> Direction.West
            | Direction.North when this.Symbol = '/' -> Direction.East
            | Direction.South when this.Symbol = '/' -> Direction.West
            | Direction.South when this.Symbol = '\\' -> Direction.East
            | Direction.West when this.Symbol = '\\' -> Direction.North
            | Direction.West when this.Symbol = '/' -> Direction.South
            | Direction.East when this.Symbol = '\\' -> Direction.South
            | Direction.East when this.Symbol = '/' -> Direction.North
            | _ -> Direction.None

    type Space(x: int, y: int) =
        inherit Symbol('.')
        member val Column = x with get
        member val Row = y with get

    type Beam(x: int, y: int) =
        inherit Symbol('#')
        member val Column = x with get
        member val Row = y with get

    type Move(direction: Direction, x: int, y: int) =
        inherit Symbol(if direction = Direction.East then '>' elif direction = Direction.North then '^' elif direction = Direction.South then 'V' else '<')
        member val Move = direction with get
        member val Row = y with get
        member val Col = x with get

    type PathTransformer(x: int, y: int, direction: Direction, layout: Symbol array array) =
        member val StartCol = x with get
        member val StartRow = y with get
        member val StartDirection = direction with get, set
        member val Layout = layout with get
        member val VisitedStartPoints = [] with get, set
        member val VisitedPaths = [] with get, set
        member val HasRun = false with get, set
        member val Width = layout[0].Length with get
        member val Height = layout.Length with get
        member val Energized = 0 with get, set
        member this.Traverse() =
            let mutable paths = [(this.StartCol, this.StartRow, this.StartDirection)]
            let mutable path = []
            while paths.Length > 0 do
                this.VisitedStartPoints <- this.VisitedStartPoints @ [paths.Head]
                let mutable (x, y, z) = paths.Head
                paths <- paths.Tail
                path <- [(x, y, z)]

                let mutable newpaths = []
                while newpaths.Length = 0 && y >=< (0, this.Height - 1) && x >=< (0, this.Width - 1) do
                    match z with
                    | Direction.East -> x <- x + 1
                    | Direction.West -> x <- x - 1
                    | Direction.South -> y <- y + 1
                    | Direction.North -> y <- y - 1
                    | _ -> ()

                    if y >=< (0, this.Height - 1) && x >=< (0, this.Width - 1) then
                        path <- path @ [(x, y, z)]
                        let symbol = this.Layout[y][x]
                        match symbol.Symbol with
                        | '.' -> 
                            symbol.Beams <- 1
                            match z with
                            | Direction.West -> symbol.Symbol <- '<'
                            | Direction.East -> symbol.Symbol <- '>'
                            | Direction.North -> symbol.Symbol <- '^'
                            | Direction.South -> symbol.Symbol <- 'V'
                            | _ -> ()
                        | '|' | '-' ->
                            let tmp = symbol :?> Splitter
                            symbol.Beams <- 1
                            let (d1, d2) = tmp.SplitTo(z, x, y)
                            match d1, d2 with
                            | Direction.None, Direction.None -> ()
                            | Direction.None, _ -> z <- d2
                            | _, Direction.None -> z <- d1
                            | _, _ ->
                                newpaths <- newpaths @ [(x, y, d1)]
                                newpaths <- newpaths @ [(x, y, d2)]
                        | '/' | '\\' ->
                            symbol.Beams <- 1
                            let tmp = symbol :?> Mirror
                            let direction = tmp.ChangeDirection(z, x, y)
                            z <- direction
                        | '>' | '<' | '^' | 'V' -> symbol.Beams <- symbol.Beams + 1
                        | _ -> ()

                newpaths
                |> List.iter (
                    fun (x, y, dir) ->
                        let item = this.VisitedStartPoints |> List.tryFind (fun (m, n, b) -> m = x && n = y && dir = b)
                        match item with
                        | None -> paths <- paths @ [(x, y, dir)]
                        | _ -> ()
                )
            this.Layout
            |> Array.map (
                fun s ->
                    s |> Array.map (fun t -> if t.Beams > 0 then 1 else 0)
            ) |> Array.concat
            |> Array.sum
            |> fun v -> this.Energized <- v
            this.HasRun <- true
    
    [<EntryPoint>]
    let main argv =
        async {
            let file = $"{argv[0]}.txt"
            let! contents = File.ReadAllLinesAsync(file) |> Async.AwaitTask

            let layout =
                contents
                |> Array.copy
                |> Array.mapi (
                    fun row line ->
                        line |> Seq.mapi (
                            fun col symbol ->
                                match symbol with
                                | '/' -> new Mirror(symbol, col, row) :> Symbol
                                | '\\' -> new Mirror(symbol, col, row) :> Symbol
                                | '-' -> new Splitter(symbol, col, row) :> Symbol
                                | '|' -> new Splitter(symbol, col, row) :> Symbol
                                | _ -> new Space(col, row) :> Symbol
                        ) |> Seq.toArray
                )
            
            let symbol = layout[0][0]
            let transformer = 
                match symbol.Symbol with
                | '/' -> new PathTransformer(0, 0, Direction.North, layout |> Array.copy)
                | '\\' ->  new PathTransformer(0, 0, Direction.South, layout |> Array.copy)
                | _ ->  new PathTransformer(0, 0, Direction.East, layout |> Array.copy)
            let threadstart = new ThreadStart(transformer.Traverse)
            let thread = new Thread(threadstart)
            thread.Start()
            thread.Join()

            transformer.Energized |> printfn "%d"

            let mutable threads = []
            for y in 0..layout.Length - 1 do
                for x in 0..layout[0].Length - 1 do
                    let layout = 
                        contents
                        |> Array.copy
                        |> Array.mapi (
                            fun row line ->
                                line |> Seq.mapi (
                                    fun col symbol ->
                                        match symbol with
                                        | '/' -> new Mirror(symbol, col, row) :> Symbol
                                        | '\\' -> new Mirror(symbol, col, row) :> Symbol
                                        | '-' -> new Splitter(symbol, col, row) :> Symbol
                                        | '|' -> new Splitter(symbol, col, row) :> Symbol
                                        | _ -> new Space(col, row) :> Symbol
                                ) |> Seq.toArray
                        )
                    if y = 0 || y = layout.Length - 1 then
                        let o = new PathTransformer(x, y, (if y = 0 then Direction.South else Direction.North), layout |> Array.copy)
                        let t = new Thread(new ThreadStart(o.Traverse))
                        threads <- threads @ [(o, t)]
                    if (x = 0 || x = layout[0].Length - 1) then
                        let o = new PathTransformer(x, y, (if x = 0 then Direction.East else Direction.West), layout |> Array.copy)
                        let t = new Thread(new ThreadStart(o.Traverse))
                        threads <- threads @ [(o, t)]

            threads |> List.iter (fun (_, t) -> t.Start())

            threads
            |> List.map (
                fun (o, t) ->
                    t.Join()
                    o.Energized
            )
            |> List.sortDescending
            |> fun p -> p[0]
            |> printfn "%d"

            return 0
        }
        |> Async.RunSynchronously