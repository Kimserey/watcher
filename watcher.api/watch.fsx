#r "../packages/Fake/tools/FakeLib.dll"

open System
open Fake
open Fake.REST


open System.IO
open System.Net
open System.Web
open System.Xml
open System.Text

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

Target "Watch" (fun _ ->
    use watcher = 
        !! "**/*.js" 
        |> WatchChanges (fun _ -> ExecutePost "http://127.0.0.1:8083/refresh" "." "." "" |> ignore)

          
    //Prevents watcher from stopping
    System.Console.ReadLine() |> ignore
    watcher.Dispose()
)

RunTargetOrDefault "Watch"