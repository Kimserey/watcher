open Suave
open Suave.Sockets
open Suave.Sockets.Control
open Suave.Http
open Suave.EventSource
open Suave.Utils
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.Files
open Suave.Web
open System
open System.IO


let (<!>) a b =
    match a with
    | Choice1Of2 x -> Choice1Of2 x
    | Choice2Of2 _ -> b

let (<.>) a b =
    match a with
    | Choice1Of2 x -> x
    | Choice2Of2 _ -> b

/// Maybe convert to int32 from string
let muint32 str =
    match System.UInt32.TryParse str with
    | true, i -> Choice1Of2 i
    | _       -> Choice2Of2 "couldn't convert to int32"


let indexPath =
    __SOURCE_DIRECTORY__ + "/index.html"

let webPart =
    choose 
        [ GET >=> choose [ path "/" >=> file indexPath
                           browseHome
                           path "/events2" >=> request (fun _ -> EventSource.handShake (fun out ->
                            socket {
                                let msg = { id = "1"; data = "First Message"; ``type`` = None }
                                do! msg |> send out
                                let msg = { id = "2"; data = "Second Message"; ``type`` = None }
                                do! msg |> send out
                              })) 
                           path "/events" >=> request (fun r -> EventSource.handShake (fun out ->
                            socket {
                                printf "hello"
                            })) ] ]

[<EntryPoint>]
let main argv = 
    startWebServer { defaultConfig with homeFolder = Some __SOURCE_DIRECTORY__ } webPart
    0
