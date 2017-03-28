module CommandApi

open CommandHandlers
open Queries
open OpenTab
open Chessie.ErrorHandling
open PlaceOrder
open ServeDrink
open ServeFood
open PrepareFood
open CloseTab

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
  | PrepareFoodRequest (tabId, foodMenuNumber) ->
    queries.Food.GetFoodByMenuNumber
    |> prepareFoodCommander queries.Table.GetTableByTabId
    |> handleCommand eventStore (tabId, foodMenuNumber)
  | ServeFoodRequest (tabId, foodMenuNumber) ->
    queries.Food.GetFoodByMenuNumber
    |> serveFoodCommander queries.Table.GetTableByTabId
    |> handleCommand eventStore (tabId, foodMenuNumber)
  | CloseTabRequest (tabId, amount) ->
    closeTabCommander queries.Table.GetTableByTabId
    |> handleCommand eventStore (tabId, amount)
  | _ -> err "Invalid command" |> fail |> async.Return