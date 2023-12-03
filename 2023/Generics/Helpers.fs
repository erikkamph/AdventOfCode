namespace Home

open System.IO
open System.Text
open Home.Types

module Helpers =
    let GetFileContents (fileName: string) =
        async {
            try
                return! File.ReadAllLinesAsync(fileName, Encoding.UTF8) |> Async.AwaitTask
            with
            | ex ->
                printfn $"{ex.Message} {ex.StackTrace}"
                return [||]
        }
    
    let GetFilter() =
        async {
            let! contents = GetFileContents @"filter.json"
            
            return
                contents
                |> String.concat "\n"
                |> Json.JsonSerializer.Deserialize<Filter>
        }


