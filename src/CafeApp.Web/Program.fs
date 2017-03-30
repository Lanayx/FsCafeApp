module Program

open Suave
open Suave.Web
open Suave.RequestErrors
open Suave.Operators
open Suave.Filters
open CommandApi
open InMemory
open System.Text
open Chessie.ErrorHandling
open Events
open Projections
open JsonFormatter
open CommandHandlers
open QueriesApi

let eventsStream = new Control.Event<Event list>()

let project event =
  projectReadModel inMemoryActions event
  |> Async.RunSynchronously |> ignore

let projectEvents = List.iter project

let toErrorJson err =
  jobj [ "error" .= err.Message]
  |> string |> JSON BAD_REQUEST

let commandApiHandler eventStore (context : HttpContext) = async {
  let payload =
    Encoding.UTF8.GetString context.request.rawForm
  let! response =
    handleCommandRequest
      inMemoryQueries eventStore payload
  match response with
  | Ok ((state,events), _) ->
    do! eventStore.SaveEvents state events
    eventsStream.Trigger(events)
    return! toStateJson state context
  | Bad (err) ->
    return! toErrorJson err.Head context
}

let commandApi eventStore =
  path "/command"
    >=> POST
    >=> commandApiHandler eventStore

[<EntryPoint>]
let main argv =
  let app =
    let eventStore = inMemoryEventStore ()
    choose [
      commandApi eventStore
      queriesApi inMemoryQueries eventStore
    ]

  eventsStream.Publish.Add(projectEvents)

  let cfg = {defaultConfig with
              bindings = [HttpBinding.createSimple HTTP "0.0.0.0" 8083]
            }
  startWebServer cfg app
  0
