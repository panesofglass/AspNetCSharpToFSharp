namespace MsdnWeb.Todo

// All Home-related types and features should go here.
// NOTE: You cannot use a module within MVC.

open System
open System.ComponentModel.DataAnnotations
open System.Web
open System.Web.Mvc

// Models

type TodoState
  = Complete
  | OnTime
  | Overdue
  with
  override x.ToString() =
    match x with
    | Complete -> "Complete"
    | OnTime -> "On-time"
    | Overdue -> "Overdue"

type TodoItem() =
  let mutable name = ""
  let mutable dueDate = Unchecked.defaultof<Nullable<DateTime>>
  let mutable completedDate = Unchecked.defaultof<Nullable<DateTime>>
  [<Required>]
  member x.Name
    with get() = name
    and set(v) = name <- v
  member x.Due
    with get() = dueDate
    and set(v) = dueDate <- v
  member x.Completed
    with get() = completedDate
    and set(v) = completedDate <- v
  [<HiddenInput(DisplayValue = false)>]
  member x.State =
      if x.Completed.HasValue then Complete
      elif not x.Due.HasValue || x.Due.Value <= DateTime.UtcNow then OnTime
      else Overdue
  member x.MarkComplete() =
    x.Completed <- new Nullable<DateTime>(DateTime.UtcNow)
  member x.Reset() =
    x.Completed <- Unchecked.defaultof<Nullable<DateTime>>

type TodoList() =
  let mutable items : TodoItem list = []
  member x.Items = items :> seq<TodoItem>
  member x.State =
    match items with
    | [] -> OnTime
    | item::[] -> item.State
    | _ -> items |> List.map (fun i -> i.State) |> List.max
  member x.Add(item) = items <- item::items
  member x.Remove(item) = items <- items |> List.filter ((<>) item)
  member x.MarkAllComplete() = for i in items do i.MarkComplete()
  member x.ResetAll() = for i in items do i.Reset()

// For the purposes of this demo and to avoid actually using a database,
// I'm adding a property to Global.
[<AutoOpen>]
module Database =
  let todoList = TodoList()
  type MsdnWeb.Global with
    static member TodoList = todoList

// Controllers

type TodoController() =
    inherit Controller()
    member this.Index () =
        this.View(MsdnWeb.Global.TodoList) :> ActionResult
    [<HttpPost; ValidateAntiForgeryToken>]
    member this.Index (item) =
        if this.ModelState.IsValid then
          todoList.Add(item)
          this.Response.StatusCode <- 201
          this.RedirectToAction("Index") :> ActionResult
        else this.View() :> ActionResult
    member this.Create () =
        this.View(new TodoItem()) :> ActionResult

// If you really want, you could even embed your views as resources
// and use the MVC Areas feature to make this truly modular.
