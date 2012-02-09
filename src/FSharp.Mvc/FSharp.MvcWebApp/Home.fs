namespace MsdnWeb.Home

// All Home-related types and features should go here.
// NOTE: You cannot use a module within MVC unless you create your own controller factory.
// NOTE: If you use an IoC container, you should be fine.

open System.Web
open System.Web.Mvc

// Models

// Controllers

type HomeController() =
    inherit Controller()
    member this.Index () =
        this.View() :> ActionResult

// If you really want, you could even embed your views as resources
// and use the MVC Areas feature to make this truly modular.
