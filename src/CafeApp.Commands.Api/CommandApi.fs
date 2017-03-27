module CommandApi

open System.Text
open CommandHandlers
open Queries
open OpenTab
open Chessie.ErrorHandling
open PlaceOrder
open ServeDrink

let handleCommandRequest queries eventStore
  = function
  | OpenTabRequest tab ->
    queries.Table.GetTableByTableNumber
    |> openTabCommander
    |> handleCommand eventStore tab
  | PlaceOrderRequest placeOrder ->
    placeOrderCommander queries
    |> handleCommand eventStore placeOrder
  | ServeDrinkRequest (tabId, drinkMenuNumber) ->
    queries.Drink.GetDrinkByMenuNumber
    |> serveDrinkCommander queries.Table.GetTableByTabId
    |> handleCommand eventStore (tabId, drinkMenuNumber)
  | _ -> err "Invalid command" |> fail |> async.Return