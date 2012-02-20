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

type TodoItemViewModel =
  { Id        : int
    Name      : string
    Due       : Nullable<DateTime>
    Completed : Nullable<DateTime>
    Status    : string }

// Controllers

type TodoController() =
  inherit Controller()

  member this.Index () =
    let todoList = Db.todoList.PostAndReply(Get)
    this.ViewData.["State"] <- todoList.State
    this.ViewData.["Items"] <- todoList.Items 
                               |> List.map (fun i ->
                                  { Id = i.Id
                                    Name = i.Name
                                    Due = Option.toNullable i.Due
                                    Completed = Option.toNullable i.Completed
                                    Status = i.State.ToString() })
    this.View() :> ActionResult

  [<HttpPost; ValidateAntiForgeryToken>]
  member this.Index(name: string, due: Nullable<DateTime>, completed: Nullable<DateTime>) =
    if not(String.IsNullOrEmpty name) then
      this.ModelState.AddModelError("Name", "An item must have a name.")
      this.View() :> ActionResult
    else
      let todoList = Db.todoList.PostAndReply(Get)
      let item = { Id = 0; Name = name; Due = Option.fromNullable due; Completed = Option.fromNullable completed }
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
  member this.Put(id: int, name: string, due: Nullable<DateTime>, completed: Nullable<DateTime>) =
    if id = 0 then
      HttpNotFoundResult() :> ActionResult
    elif not(String.IsNullOrEmpty name) then
      this.ModelState.AddModelError("Name", "An item must have a name.")
      this.View() :> ActionResult
    else
      let item = { Id = id; Name = name; Due = due |> toOption; Completed = completed |> toOption }
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
