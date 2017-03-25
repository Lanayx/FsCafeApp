module CommandApi

open System.Text
open CommandHandlers
open Queries
open OpenTab
open Chessie.ErrorHandling

let handleCommandRequest queries eventStore
  = function
  | OpenTabRequest tab ->
    queries.Table.GetTableByTableNumber
    |> openTabCommander
    |> handleCommand eventStore tab
  | _ -> err "Invalid command" |> fail |> async.Return