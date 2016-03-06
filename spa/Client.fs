namespace spa

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client

[<JavaScript>]
module Client =    

    let main =
        div [ text "Hello world" ]
        |> Doc.RunById "main"
