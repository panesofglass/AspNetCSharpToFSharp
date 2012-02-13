namespace Todo

open System

type TodoState
  = Complete
  | OnTime
  | Overdue

type internal TodoRecord =
  { mutable Name: string
    mutable Due: DateTime option
    mutable Completed: DateTime option }

[<System.ComponentModel.Composition.Export>]
type TodoItem internal (item: TodoRecord) =
  new () = TodoItem { Name = ""; Due = None; Completed = None }
  member x.Name
    with get() = item.Name
    and set(v) = item.Name <- v
  member x.Due
    with get() = item.Due
    and set(v) = item.Due <- Some v
  member x.Completed
    with get() = item.Completed
    and set(v) = item.Completed <- Some v
  member x.State =
    match item with
    | { Completed = Some _ } -> Complete
    | { Due = Some due } when due <= DateTime.UtcNow -> OnTime
    | _ -> Overdue
  member x.MarkComplete() =
    item.Completed <- Some DateTime.UtcNow
  member x.Reset() =
    item.Completed = None

type TodoListMessage
  = Items of AsyncReplyChannel<TodoItem list>
  | State of AsyncReplyChannel<TodoState>
  | Add of TodoItem
  | Remove of TodoItem
  | MarkAllComplete
  | ResetAll

[<System.ComponentModel.Composition.Export>]
type TodoList() =
  let agent = MailboxProcessor.Start(fun inbox ->
    let rec loop list = async {
      let! msg = inbox.Receive()
      match msg with
      | Items(reply)    -> reply.Reply(list)
                           return! loop list
      | State(reply)    -> let state = match list with
                                       | [] -> OnTime
                                       | item::[] -> item.State
                                       | _ -> list |> List.map (fun i -> i.State) |> List.max
                           reply.Reply(state)
                           return! loop list
      | Add(item)       -> return! loop <| item::list
      | Remove(item)    -> return! loop (list |> List.filter ((<>) item))
      | MarkAllComplete -> return! loop (list |> List.map (fun i -> i.MarkComplete()))
      | ResetAll        -> return! loop (list |> List.map (fun i -> i.Reset()))
    }
    loop [] )

  member x.Items = agent.PostAndReply(Items)
  member x.State = agent.PostAndReply(State)
  member x.Add(item) = agent.Post(Add item)
  member x.Remove(item) = agent.Post(Remove item)
  member x.MarkAllComplete() = agent.Post(MarkAllComplete)
  member x.ResetAll() = agent.Post(ResetAll)
