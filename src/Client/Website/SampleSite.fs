﻿namespace Website

open System
open System.IO
open System.Web
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets

type HelloWorldData = { Message: string }

module Client =
    open IntelliFactory.WebSharper.Html
    open IntelliFactory.WebSharper.JQuery

    [<JavaScript>]
    let HelloWorld() =
        Div [Id "message"] -< []
        |>! OnAfterRender (fun self ->
            JQuery.GetJSON("HelloWorld", fun (data,_) ->
                self.Text <- data.Message)
            |> ignore)

[<Sealed>]
type HelloWorldControl() =
    inherit Web.Control()

    [<JavaScript>]
    override this.Body = Client.HelloWorld() :> _

/// The website definition.
module SampleSite =
    open IntelliFactory.Html

    /// Actions that corresponds to the different pages in the site.
    type Action =
        | Home
        | Contact
        | Protected
        | Login of option<Action>
        | Logout
        | Echo of string
        | HelloWorld
        | Message

    /// A helper function to create a hyperlink
    let private ( => ) title href =
        A [HRef href] -< [Text title]

    /// A helper function to create a 'fresh' url with a random get parameter
    /// in order to make sure that browsers don't show a cached version.
    let private RandomizeUrl url =
        url + "?d=" + System.Uri.EscapeUriString (System.DateTime.Now.ToString())

    /// User-defined widgets.
    module Widgets =

        /// Widget for displaying login status or a link to login.
        let LoginInfo (ctx: Context<Action>) =
            let user = UserSession.GetLoggedInUser ()
            [
                (
                    match user with
                    | Some email ->
                        "Log Out (" + email + ")" => 
                            (RandomizeUrl <| ctx.Link Action.Logout)
                    | None ->
                        "Login" => (ctx.Link <| Action.Login None)
                )
            ]

    /// A template function that renders a page with a menu bar, based on the `Skin` template.
    let Template title main : Content<Action> =
        let menu (ctx: Context<Action>)=
            let ( ! ) x = ctx.Link x
            [
                    "Home"      => !Action.Home
                    "Contact"   => !Action.Contact
                    "Say Hello" => !(Action.Echo "Hello")
                    "Message"   => !Action.Message
                    "Protected" => (RandomizeUrl <| !Action.Protected)
                    "ASPX Page" => ctx.ResolveUrl "~/LegacyPage.aspx"
            ]
            |> List.map (fun link -> 
                Label [Class "menu-item"] -< [link]
            )
        Templates.Main.Main (Some title)
            {
                LoginInfo   = Widgets.LoginInfo
                Banner      = fun ctx -> [H2 [Text title]]
                Menu        = menu
                Main        = main
                Sidebar     = fun ctx -> [Text "Put your side bar here"]
                Footer      = fun ctx -> [Text "Your website.  Copyright (c) 2011 YourCompany.com"]
            }

    module Apis =
        let HelloWorld : Content<Action> =
            CustomContent <| fun ctx ->
                {
                    Status = Http.Status.Ok
                    Headers = [ Http.Header.Custom "Content-Type" "application/json" ]
                    WriteBody = fun stream ->
                        use tw = new System.IO.StreamWriter(stream)
                        tw.WriteLine "{\"Message\": \"hello, world!\"}"
                }

    /// The pages of this website.
    module Pages =

        let Message : Content<Action> =
            Template "Message" <| fun ctx ->
                [
                    H1 [Text "Message"]
                    Div [new HelloWorldControl()]
                ]

        /// The home page.
        let HomePage : Content<Action> =
            Template "Home" <| fun ctx ->
                [
                    H1 [Text "Welcome to our site!"]
                    "Let us know how we can contact you" => ctx.Link Action.Contact
                 ]

        /// A page to collect contact information.
        let ContactPage : Content<Action> =
            Template "Contact" <| fun ctx ->
                [
                    H1 [Text "Contact Form"]
                    Div [new SignupSequenceControl()]
                ]

        /// A simple page that echoes a parameter.
        let EchoPage param : Content<Action> =
            Template "Echo" <| fun ctx ->
                [
                    H1 [Text param]
                ]

        /// A simple login page.
        let LoginPage (redirectAction: option<Action>): Content<Action> =
            Template "Login" <| fun ctx ->
                let redirectLink =
                    match redirectAction with
                    | Some action -> action
                    | None        -> Action.Home
                    |> ctx.Link
                [
                    H1 [Text "Login"]
                    P [
                        Text "Login with any username and password='"
                        I [Text "password"]
                        Text "'."
                    ]
                    Div [
                        new LoginControl(redirectLink)
                    ]
                ]

        /// A simple page that users must log in to view.
        let ProtectedPage : Content<Action> =
            Template "Protected" <| fun ctx ->
                [
                    H1 [Text "This is protected content - thanks for logging in!"]
                ]

    /// The sitelet that corresponds to the entire site.
    let EntireSite =
        // A simple sitelet for the home page, available at the root of the application.
        let home = 
            Sitelet.Content "/" Action.Home Pages.HomePage

        let api =
            Sitelet.Content "/api" Action.HelloWorld Apis.HelloWorld

        // An automatically inferred sitelet created for the basic parts of the application.
        let basic =
            Sitelet.Infer <| fun action ->
                match action with
                | Action.Contact ->
                    Pages.ContactPage
                | Action.Echo param ->
                    Pages.EchoPage param
                | Action.HelloWorld ->
                    Apis.HelloWorld
                | Action.Message ->
                    Pages.Message
                | Action.Login action->
                    Pages.LoginPage action
                | Action.Logout ->
                    // Logout user and redirect to home
                    UserSession.Logout ()
                    Content.Redirect Action.Home
                | Action.Home ->
                    Content.Redirect Action.Home
                | Action.Protected ->
                    Content.ServerError

        // A sitelet for the protected content that requires users to log in first.
        let authenticated =
            let filter : Sitelet.Filter<Action> =
                {
                    VerifyUser = fun _ -> true
                    LoginRedirect = Some >> Action.Login
                }

            Sitelet.Protect filter <|
                Sitelet.Content "/protected" Action.Protected Pages.ProtectedPage

        // Compose the above sitelets into a larger one.
        [
            home
            api
            authenticated
            basic
        ]
        |> Sitelet.Sum

/// Expose the main sitelet so that it can be served.
/// This needs an IWebsite type and an assembly level annotation.
type SampleWebsite() =
    interface IWebsite<SampleSite.Action> with
        member this.Sitelet = SampleSite.EntireSite
        member this.Actions = []

[<assembly: WebsiteAttribute(typeof<SampleWebsite>)>]
do ()
