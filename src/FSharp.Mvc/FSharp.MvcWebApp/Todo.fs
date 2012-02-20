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
    let item =
      if item.Id = 0 then
        { item with Id = (x.Items |> List.map (fun i -> i.Id) |> List.max) + 1 }
      else item
    { x with Items = item::x.Items }

  member x.Remove(item) =
    { x with Items = x.Items |> List.filter ((<>) item) }

// For the purposes of this demo and to avoid actually using a database,
// I'm adding a property to Global.
type TodoAction
  = Get of AsyncReplyChannel<TodoList>
  | Add of TodoItem
  | Remove of TodoItem
  
module Db =
  let todoList = MailboxProcessor<TodoAction>.Start(fun inbox ->
    let todos = { Items = [] }
    let rec loop todos = async {
      let! msg = inbox.Receive()
      match msg with
      | Get reply ->
          reply.Reply todos
          return! loop todos
      | Add item -> return! loop <| todos.Add item
      | Remove item -> return! loop <| todos.Remove item }
    loop todos)

// View Models (only for MVC)

type TodoItemViewModel() =
  let mutable id = 0
  let mutable name : string = null
  let mutable due = Unchecked.defaultof<Nullable<DateTime>>
  let mutable completed = Unchecked.defaultof<Nullable<DateTime>>
  let mutable status : string = null
  member x.Id with get() = id and set(v) = id <- v
  [<Required>]
  member x.Name with get() = name and set(v) = name <- v
  member x.Due with get() = due and set(v) = due <- v
  member x.Completed with get() = completed and set(v) = completed <- v
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
    let todoList = Db.todoList.PostAndReply(Get)
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
      let todoList = Db.todoList.PostAndReply(Get)
      let item : TodoItem = { Id = 0; Name = item.Name; Due = Option.fromNullable item.Due; Completed = Option.fromNullable item.Completed }
      Db.todoList.Post(Add item)
      this.Response.StatusCode <- 201
      this.RedirectToAction("Index") :> ActionResult

  member this.Create () =
    this.View() :> ActionResult


type TodoItemController() =
  inherit Controller()

  member this.Get() =
    EmptyResult() :> ActionResult

  [<HttpPut; ValidateAntiForgeryToken>]
  member this.Put(item: TodoItemViewModel) =
    if item.Id = 0 then
      HttpNotFoundResult() :> ActionResult
    elif not this.ModelState.IsValid then
      this.View(item) :> ActionResult
    else
      let item : TodoItem = { Id = item.Id; Name = item.Name; Due = item.Due |> Option.fromNullable; Completed = item.Completed |> Option.fromNullable }
      let todoList = Db.todoList.PostAndReply(Get)
      match todoList.Items |> List.tryFind (fun i -> i.Id = item.Id) with
      | Some(oldItem) ->
          Db.todoList.Post(Remove oldItem)
          Db.todoList.Post(Add item)
          this.RedirectToAction("Index", "Todo") :> ActionResult
      | _ -> HttpNotFoundResult() :> ActionResult

  [<HttpDelete; ValidateAntiForgeryToken>]
  member this.Delete(id) =
    let todoList = Db.todoList.PostAndReply(Get)
    match todoList.Items |> List.tryFind (fun i -> i.Id = id) with
    | Some(item) ->
        Db.todoList.Post(Remove item)
        this.RedirectToAction("Index", "Todo") :> ActionResult
    | _ -> HttpNotFoundResult() :> ActionResult

// If you really want, you could even embed your views as resources
// and use the MVC Areas feature to make this truly modular.
