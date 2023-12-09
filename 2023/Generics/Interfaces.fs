namespace AdventOfCode

open System.Threading.Tasks

module Interfaces =
    type Mutations =
        abstract member IsInCombination: string*string list -> Async<bool>
        abstract member Mutate: unit -> Async<string>
        abstract member CalculateMutations: unit -> Async<int>

    type FunctionsRunner =
        abstract member Run: unit -> Task<int array>