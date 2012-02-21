namespace MsdnWeb

open System
open System.Web
open System.Web.Mvc
open System.Web.Routing

type Route = { controller : string
               action : string
               id : UrlParameter }

type Global() =
    inherit System.Web.HttpApplication() 

    static member RegisterRoutes(routes:RouteCollection) =
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")
        routes.MapRoute("Item", "item/{id}", { controller = "TodoItem"; action = "Index"; id = UrlParameter.Optional } ) |> ignore
        routes.MapRoute("Default", "{action}/{id}", { controller = "Todo"; action = "Index"; id = UrlParameter.Optional } ) |> ignore

    member this.Start() =
        AreaRegistration.RegisterAllAreas()
        GlobalFilters.Filters.Add(new HandleErrorAttribute())
        Global.RegisterRoutes(RouteTable.Routes)
