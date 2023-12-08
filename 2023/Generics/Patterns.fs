namespace AdventOfCode
open Types

module Patterns =
    let (|FiveOfAKind|_|) (h: Hand) =
        let length = h.CountOfC.Length
        let (_, n) = h.CountOfC[0]
        match length, n with
        | 1, 5 -> Some(h)
        | _, _ -> None
    
    let (|FourOfAKind|_|) (h: Hand) =
        let length = h.CountOfC.Length
        let (_, n) = h.CountOfC[0]
        match length, n with
        | 2, 4 -> Some(h)
        | _, _ -> None
    
    let (|FullHouse|_|) (h: Hand) =
        let length = h.CountOfC.Length
        let (_, n) = h.CountOfC[0]
        let (_, m) = h.CountOfC[1]
        match length, n, m with
        | 2, 2, 3 | 2, 3, 2 -> Some(h)
        | _, _, _ -> None
    
    let (|ThreeOfAKind|_|) (h: Hand) =
        let length = h.CountOfC.Length
        let (_, n) = h.CountOfC[0]
        match length, n with
        | 3, 3 -> Some(h)
        | _, _ -> None
    
    let (|TwoPair|_|) (h: Hand) =
        let length = h.CountOfC.Length
        let (_, x) = h.CountOfC[0]
        let (_, y) = h.CountOfC[1]
        match length, x, y with
        | 3, 2, 2 -> Some(h)
        | _, _, _ -> None
    
    let (|OnePair|_|) (h: Hand) =
        let length = h.CountOfC.Length
        let (_, x) = h.CountOfC[0]
        match length, x with
        | 4, 2 -> Some(h)
        | _, _ -> None
    
    let (|HighCard|_|) (h: Hand) =
        match h.CountOfC.Length with
        | 5 -> Some(h)
        | _ -> None