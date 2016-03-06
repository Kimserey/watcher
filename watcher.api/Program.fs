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
| Query of AsyncReplyChannel<bool * int>

let agent = MailboxProcessor.Start (fun inbox ->
        let rec loop state =
            async {
                let! msg = inbox.Receive()
                let updatedState =
                    match state, msg with
                    | (_, id),       Order -> true, id
                    | (refresh, id), Query channel when not refresh -> channel.Reply state; state
                    | (refresh, id), Query channel when refresh     -> channel.Reply state; false, id + 1
                    | _ -> state
                return! loop updatedState
            }
        loop (false, 0))

let app =
    choose 
        [ GET >=> choose [ browseHome
                           path "/"       >=> file (__SOURCE_DIRECTORY__ + "/index.html")
                           path "/events" >=> request (fun _ -> 
                            EventSource.handShake (fun out ->
                                socket {
                                    while true do
                                        do! SocketOp.ofAsync(Async.Sleep(500))
                                        let! (refresh, id) = SocketOp.ofAsync(agent.PostAndAsyncReply Query)

                                        if refresh then
                                            do! send out (mkMessage (string id) "refresh")
                                })) ]
          POST >=> path "/refresh"  >=> request (fun _ -> agent.Post Order; OK "Refresh ordered.") ]             


[<EntryPoint>]
let main argv = 
    startWebServer { defaultConfig with homeFolder = Some __SOURCE_DIRECTORY__ } app
    0
