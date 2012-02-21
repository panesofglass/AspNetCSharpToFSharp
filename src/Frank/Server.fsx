(* # Frank Self-Host Sample

## License

Author: Ryan Riley <ryan.riley@panesofglass.org>
Copyright (c) 2010-2011, Ryan Riley.

Licensed under the Apache License, Version 2.0.
See LICENSE.txt for details.
*)

#r "System.ServiceModel"
#r "System.ServiceModel.Web"
#r @"..\..\packages\FSharpx.Core.1.3.111030\lib\FSharpx.Core.dll"
#r @"..\..\packages\FSharpx.Core.1.3.111030\lib\FSharpx.Http.dll"
#r @"..\..\packages\HttpClient.0.6.0\lib\40\System.Net.Http.dll"
#r @"..\..\packages\HttpClient.0.6.0\lib\40\Microsoft.Net.Http.Formatting.dll"
#r @"..\..\packages\WebApi.0.6.0\lib\40-Full\Microsoft.Runtime.Serialization.Internal.dll"
#r @"..\..\packages\WebApi.0.6.0\lib\40-Full\Microsoft.ServiceModel.Internal.dll"
#r @"..\..\packages\WebApi.0.6.0\lib\40-Full\Microsoft.Server.Common.dll"
#r @"..\..\packages\WebApi.0.6.0\lib\40-Full\Microsoft.ApplicationServer.Http.dll"
#r @"..\..\packages\WebApi.Enhancements.0.6.0\lib\40-Full\Microsoft.ApplicationServer.HttpEnhancements.dll"
#r @"..\..\packages\Frank.0.6.120122\lib\Frank.dll"

open System.Net
open System.Net.Http
open Frank
open Frank.Hosting

let formatters = [| new Formatting.JsonMediaTypeFormatter() :> Formatting.MediaTypeFormatter
                    new Formatting.XmlMediaTypeFormatter() :> Formatting.MediaTypeFormatter |]

// Respond with a web page containing "Hello, world!" and a form submission to use the POST method of the resource.
let helloWorld request = async {
  return HttpResponseMessage.ReplyTo(request, new StringContent(@"<!doctype html>
<meta charset=utf-8>
<title>Hello</title>
<p>Hello, world!
<form action=""/"" method=""post"">
<input type=""hidden"" name=""text"" value=""testing"">
<input type=""submit"">", System.Text.Encoding.UTF8, "text/html"), ``Content-Type`` "text/html")
}

// Respond with the request content, if any.
let echo = negotiateMediaType formatters <| fun request -> request.Content.AsyncReadAsString()

let resource = route "/" (get helloWorld <|> post echo)

// Mount the app and add a middleware to support HEAD requests.
let app = merge [ resource ] //|> Middleware.head

let config = WebApi.configure app
let baseUri = "http://localhost:1000/"
let host = new Microsoft.ApplicationServer.Http.HttpServiceHost(typeof<WebApi.FrankApi>, config, [| baseUri |])
host.Open()

printfn "Host open.  Hit enter to exit..."
printfn "Use a web browser and go to %A or do it right and get fiddler!" baseUri
System.Console.Read() |> ignore

host.Close()

