#r "../packages/Fake/tools/FakeLib.dll"

open System
open Fake
open Fake.MSBuild
open Fake.REST
open Fake.FileHelper

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

Target "Watch" (fun _ ->
    use watcher = 
        !! "**/*.js" 
        |> WatchChanges (fun _ -> 
            let setParams defaults =
                { defaults with
                    Verbosity = Some(Quiet)
                    Targets = ["Build"]
                    Properties =
                        [
                            "Optimize", "True"
                            "DebugSymbols", "False"
                        ] }
            
            //Builds solution to recompile WebSharper spa.
            build setParams "../watcher.sln"

            //Copies the static files to /site folder.
            CopyDir "." "../spa/Content" (fun _ -> true)
            CopyFile "." "../spa/index.html"

            //Sends a POST request to refresh the browser.
            ExecutePost "http://127.0.0.1:8083/refresh" "." "." "" |> ignore)

          
    //Prevents watcher from stopping
    System.Console.ReadLine() |> ignore
    watcher.Dispose()
)

RunTargetOrDefault "Watch"
