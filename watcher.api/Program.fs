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

let indexPath =
    __SOURCE_DIRECTORY__ + "/index.html"


type Refresh =
| Order
| Query of AsyncReplyChannel<bool>

let agent = MailboxProcessor.Start (fun inbox ->
        let rec loop state =
            async {
                let! msg = inbox.Receive()
                match msg with
                | Order -> 
                    return! loop true
                | Query channel ->
                    do channel.Reply state
                    return! loop false
            }
        loop false)

let app =
    choose 
        [ GET >=> choose [ path "/"       >=> file (__SOURCE_DIRECTORY__ + "/index.html")
                           path "/events" >=> request (fun _ -> 
                            EventSource.handShake (fun out ->
                                socket {
                                    while true do
                                        do! SocketOp.ofAsync(Async.Sleep(2000))
                                        let! refresh = SocketOp.ofAsync(agent.PostAndAsyncReply Query)

                                        if refresh then
                                            do! send out (mkMessage (Guid.NewGuid().ToString()) "refresh")
                                })) ]
          POST >=> path "/refresh"  >=> request (fun _ -> printfn "Refresh requested"; agent.Post Order; OK "Refresh ordered.") ]             


[<EntryPoint>]
let main argv = 
    startWebServer { defaultConfig with homeFolder = Some __SOURCE_DIRECTORY__ } app
    0
