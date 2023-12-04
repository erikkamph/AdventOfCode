namespace Home

module Types =
    type Colors = { Red: int; Green: int; Blue: int }
    type Filter = { Colors: Colors }

    type CoordinatedSymbol() =
        member val Symbol: char = '.' with get, set
        member val Row: int = 0 with get, set
        member val Col: int = 0 with get, set
        member val IsDot: bool = false with get, set
        member val AdjacentValues: int list = [] with get, set
        member val AdjacentCoords: (int*int) list = List.empty<int*int> with get, set
        member val Sum: int = 0 with get, set
    
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