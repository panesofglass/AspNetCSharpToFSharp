namespace Website

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Formlet
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.Sitelets
open IntelliFactory.WebSharper.Web

/// This module defines the client-side functionality used by the website.
module Forms =

    type LoginInfo =
        {
            Name : string
            Password : string
        }
    type BasicInfo =
        {
            Name : string
            Age : int
        }

    type Address =
        {
            Street : string
            City : string
            Country : string
        }

    type Contact =
        | PhoneContact of string
        | AddressContact of Address

    [<JavaScript>]
    let Input (label: string) (err: string) =
        Controls.Input ""
        |> Validator.IsNotEmpty err
        |> Enhance.WithValidationIcon
        |> Enhance.WithTextLabel label

    [<JavaScript>]
    let InputInt (label: string) (err: string) =
        Controls.Input ""
        |> Validator.IsInt err
        |> Enhance.WithValidationIcon
        |> Enhance.WithTextLabel label
        |> Formlet.Map int

    [<JavaScript>]
    let BasicInfoForm () : Formlet<BasicInfo> =
        Formlet.Yield (fun name age -> { Name = name; Age = age })
        <*> Input "Name" "Please enter your name"
        <*> InputInt "Age" "Please enter a valid age"

    [<JavaScript>]
    let ContactInfoForm () =
        let phone =
            Input "Phone" "Empty phone number not allowed"
            |> Formlet.Map PhoneContact
        let address =
            Formlet.Yield (fun str cty ctry ->
                AddressContact {
                    Street = str
                    City = cty
                    Country = ctry
                }
            )
            <*> Input "Street" "Empty street not allowed"
            <*> Input "City" "Empty city not allowed"
            <*> Input "Country" "Empty country not allowed"

        Formlet.Do {
            let! via =
                [
                    "Phone", phone
                    "Address", address
                ]
                |> Controls.Select 0
                |> Enhance.WithTextLabel "Contact Method"
            return! via
        }

    [<JavaScript>]
    let SignupSequence () =
        let infoForm =
            BasicInfoForm ()
            |> Enhance.WithSubmitAndResetButtons
            |> Enhance.WithCustomFormContainer {
                 Enhance.FormContainerConfiguration.Default with
                    Header =
                        "Step 1 - Your name and age"
                        |> Enhance.FormPart.Text
                        |> Some
                    Description =
                        "Please enter your name and age below."
                        |> Enhance.FormPart.Text
                        |> Some
               }
        let contactForm =
            ContactInfoForm ()
            |> Enhance.WithSubmitAndResetButtons
            |> Enhance.WithCustomFormContainer {
                 Enhance.FormContainerConfiguration.Default with
                    Header =
                        "Step 2 - Your preferred contact information"
                        |> Enhance.FormPart.Text
                        |> Some
                    Description =
                        "Please enter your phone number or your address below."
                        |> Enhance.FormPart.Text
                        |> Some
               }
        let proc info contact () =
            let result =
                match contact with
                | AddressContact address ->
                    "the address: " + address.Street + ", " +
                    address.City + ", " + address.Country
                | PhoneContact phone ->
                    "the phone number: " + phone
            FieldSet [
                Legend [Text "Sign-up summary"]
                P ["Hi " + info.Name + "!" |> Text]
                P ["You are " + string info.Age + " years old" |> Text]
                P ["Your preferred contact method is via " + result |> Text]
            ]
        Formlet.Do {
            let! i = infoForm
            let! c = contactForm
            return! Formlet.OfElement (proc i c)
        }
        |> Formlet.Flowlet

    [<JavaScript>]
    let WarningPanel label =
        Formlet.Do {
            let! _ =
                Formlet.OfElement <| fun _ ->
                    Div [Attr.Class "warningPanel"] -< [Text label]
            return! Formlet.Never ()
        }

    [<JavaScript>]
    let WithLoadingPane (a: Async<'T>) (f: 'T -> Formlet<'U>) : Formlet<'U> =
        let loadingPane =
            Formlet.BuildFormlet <| fun _ ->
                let elem = 
                    Div [Attr.Class "loadingPane"]
                let state = new Event<Result<'T>>()
                async {
                    let! x = a
                    do state.Trigger (Result.Success x)
                    return ()
                }
                |> Async.Start
                elem, ignore, state.Publish
        Formlet.Replace loadingPane f
    
    [<Inline "window.location = $url">]
    let Redirect (url: string) = ()

    [<Rpc>]
    let Login (loginInfo: LoginInfo) =
        System.Threading.Thread.Sleep 1000
        if loginInfo.Password = "password" then
            UserSession.LoginUser loginInfo.Name
            true
        else
            false
        |> async.Return

    [<JavaScript>]
    let LoginForm (redirectUrl: string) : Formlet<unit> =
        let uName =
            Controls.Input ""
            |> Validator.IsNotEmpty "Enter Username"
            |> Enhance.WithValidationIcon
            |> Enhance.WithTextLabel "Username"
        let pw =
            Controls.Password ""
            |> Validator.IsNotEmpty "Enter Password"
            |> Enhance.WithValidationIcon
            |> Enhance.WithTextLabel "Password"
        let loginF =
            Formlet.Yield (fun n pw -> {Name=n; Password=pw})
            <*> uName <*> pw

        Formlet.Do {
            let! uInfo = 
                loginF
                |> Enhance.WithCustomSubmitAndResetButtons
                    {Enhance.FormButtonConfiguration.Default with Label = Some "Login"}
                    {Enhance.FormButtonConfiguration.Default with Label = Some "Reset"}
            return!
                WithLoadingPane (Login uInfo) <| fun loggedIn ->
                    if loggedIn then
                        Redirect redirectUrl
                        Formlet.Return ()
                    else
                        WarningPanel "Login failed"
        }
        |> Enhance.WithFormContainer

/// Exposes the signup form so that it can be used in sitelets.
type SignupSequenceControl() =
    inherit IntelliFactory.WebSharper.Web.Control ()

    [<JavaScript>]
    override this.Body = Forms.SignupSequence () :> _

/// Exposes the signup form so that it can be used in sitelets.
type LoginControl(redirectUrl: string) =
    inherit IntelliFactory.WebSharper.Web.Control ()

    new () = new LoginControl("?")
    [<JavaScript>]
    override this.Body = Forms.LoginForm redirectUrl :> _
