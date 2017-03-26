module CommandApi

open System.Text
open CommandHandlers
open Queries
open OpenTab
open Chessie.ErrorHandling
open PlaceOrder

let handleCommandRequest queries eventStore
  = function
  | OpenTabRequest tab ->
    queries.Table.GetTableByTableNumber
    |> openTabCommander
    |> handleCommand eventStore tab
  | PlaceOrderRequest placeOrder ->
    placeOrderCommander queries
    |> handleCommand eventStore placeOrder
  | _ -> err "Invalid command" |> fail |> async.Return