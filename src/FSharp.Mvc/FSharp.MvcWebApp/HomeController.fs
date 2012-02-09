namespace MsdnWeb.Controllers

open System.Web
open System.Web.Mvc

type HomeController() =
    inherit Controller()
    member this.Index () =
        this.View() :> ActionResult
