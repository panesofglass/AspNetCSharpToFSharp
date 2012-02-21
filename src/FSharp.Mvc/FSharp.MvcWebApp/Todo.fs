namespace MsdnWeb.Todo

// All Home-related types and features should go here.
// NOTE: You cannot use a module within MVC.

open System
open System.ComponentModel.DataAnnotations
open System.Web
open System.Web.Mvc

// Helpers

module Option =
  let fromNullable (a: Nullable<_>) =
    if a.HasValue then Some(a.Value) else None
  let toNullable = function Some v -> new Nullable<_>(v)
                          | _ -> Unchecked.defaultof<Nullable<_>>
    

// Models

type TodoState
  = Complete
  | OnTime
  | Overdue
  with
  override x.ToString() =
    match x with
    | Complete -> "Complete"
    | OnTime   -> "On-time"
    | Overdue  -> "Overdue"

type TodoItem =
  { Id : int
    Name : string
    Due : DateTime option
    Completed : DateTime option }
  with 
  member x.State =
    match x with
    | { Completed = Some _ } -> Complete
    | { Due = None } -> OnTime
    | { Due = Some due } when due <= DateTime.UtcNow.AddDays(1.) -> OnTime
    | _ -> Overdue

type TodoList =
  { Items : TodoItem list }
  with  
  member x.State =
    match x.Items with
    | [] -> OnTime
    | item::[] -> item.State
    | _ -> x.Items |> List.map (fun i -> i.State) |> List.max

  member x.Add(item) =
    let item' =
      if item.Id = 0 then
        match x.Items with
        | [] -> { item with Id = 1 }
        | _ -> { item with Id = (x.Items |> List.map (fun i -> i.Id) |> List.max) + 1 }
      else item
    { x with Items = item'::x.Items }

  member x.Remove(item) =
    { x with Items = x.Items |> List.filter ((<>) item) }

// For the purposes of this demo and to avoid actually using a database,
// I'm adding a property to Global.
type TodoAction
  = Get of AsyncReplyChannel<TodoList>
  | Add of TodoItem * AsyncReplyChannel<unit>
  | Remove of TodoItem * AsyncReplyChannel<unit>
  
type Db() =
  let todoList = MailboxProcessor<TodoAction>.Start(fun inbox ->
    let todos = { Items = [] }
    let rec loop todos = async {
      let! msg = inbox.Receive()
      match msg with
      | Get reply ->
          reply.Reply todos
          return! loop todos
      | Add(item, reply) ->
          let todos = todos.Add item
          reply.Reply()
          return! loop todos
      | Remove(item, reply) ->
          let todos = todos.Remove item
          reply.Reply()
          return! loop todos }
    loop todos)
  member x.Get() = todoList.PostAndReply(Get)
  member x.Add(item) = todoList.PostAndReply(fun c -> Add(item, c))
  member x.Remove(item) = todoList.PostAndReply(fun c -> Remove(item, c))

// Access the "database" via closure.
module Server =
  let db = Db()

// View Models (only for MVC)

type TodoItemViewModel() =
  let mutable id = 0
  let mutable name : string = null
  let mutable due = Unchecked.defaultof<Nullable<DateTime>>
  let mutable completed = Unchecked.defaultof<Nullable<DateTime>>
  let mutable status : string = null
  [<HiddenInput(DisplayValue = false)>]
  member x.Id with get() = id and set(v) = id <- v
  [<Required>]
  member x.Name with get() = name and set(v) = name <- v
  member x.Due with get() = due and set(v) = due <- v
  member x.Completed with get() = completed and set(v) = completed <- v
  [<HiddenInput(DisplayValue = false)>]
  member x.Status with get() = status and set(v) = status <- v

type TodoListViewModel() =
  let mutable items : seq<TodoItemViewModel> = Seq.empty
  let mutable status : string = null
  member x.Items with get() = items and set(v) = items <- v
  member x.Status with get() = status and set(v) = status <- v

// Controllers

type TodoController() =
  inherit Controller()

  member this.Index () =
    let todoList = Server.db.Get()
    let model = TodoListViewModel()
    model.Items <- todoList.Items 
                   |> Seq.map (fun i ->
                      new TodoItemViewModel(Id = i.Id,
                                            Name = i.Name,
                                            Due = Option.toNullable i.Due,
                                            Completed = Option.toNullable i.Completed,
                                            Status = i.State.ToString()))
    model.Status <- todoList.State.ToString()
    this.View(model) :> ActionResult

  [<HttpPost; ValidateAntiForgeryToken>]
  member this.Index(item: TodoItemViewModel) =
    if not this.ModelState.IsValid then
      this.View(item) :> ActionResult
    else
      let todoList = Server.db.Get()
      let item : TodoItem = { Id = 0; Name = item.Name; Due = Option.fromNullable item.Due; Completed = Option.fromNullable item.Completed }
      Server.db.Add(item)
      this.Response.StatusCode <- 201
      this.RedirectToAction("Index") :> ActionResult

  member this.Create () =
    this.View(new TodoItemViewModel()) :> ActionResult


type TodoItemController() =
  inherit Controller()

  [<HttpPut; ValidateAntiForgeryToken>]
  member this.Index(id: int, item: TodoItemViewModel) =
    if item.Id = 0 then
      HttpNotFoundResult() :> ActionResult
    elif not this.ModelState.IsValid then
      this.View(item) :> ActionResult
    else
      let item : TodoItem = { Id = item.Id; Name = item.Name; Due = item.Due |> Option.fromNullable; Completed = item.Completed |> Option.fromNullable }
      let todoList = Server.db.Get()
      match todoList.Items |> List.tryFind (fun i -> i.Id = item.Id) with
      | Some(oldItem) ->
          Server.db.Remove(oldItem)
          Server.db.Add(item)
          this.RedirectToAction("Index", "Todo") :> ActionResult
      | _ -> HttpNotFoundResult() :> ActionResult

  [<HttpDelete; ValidateAntiForgeryToken>]
  member this.Index (id: int) =
    let todoList = Server.db.Get()
    match todoList.Items |> List.tryFind (fun i -> i.Id = id) with
    | Some(item) ->
        Server.db.Remove(item)
        this.RedirectToAction("Index", "Todo") :> ActionResult
    | _ -> HttpNotFoundResult() :> ActionResult

// If you really want, you could even embed your views as resources
// and use the MVC Areas feature to make this truly modular.
