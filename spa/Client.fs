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
        [ div [ h1 [ text "This is a test of auto refresh" ]
                p  [ text "Hello world!" ]
                Doc.Button "Click" [] ignore ] ]
        |> Seq.cast
        |> Doc.Concat
        |> Doc.RunById "main"
