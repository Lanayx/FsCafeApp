module Projections
open Events
open Domain
open System

type TableActions = {
  OpenTab : Tab -> Async<unit>
  ReceivedOrder : Guid -> Async<unit>
}

type ChefActions = {
  AddFoodsToPrepare : Guid -> Food list -> Async<unit>
}

type WaiterActions = {
  AddDrinksToServe : Guid -> Drink list -> Async<unit>
  MarkDrinkServed : Guid -> Drink -> Async<unit>
}

type ProjectionActions = {
  Table : TableActions
  Waiter : WaiterActions
  Chef : ChefActions
}

let projectReadModel actions = function
| TabOpened tab ->
  [actions.Table.OpenTab tab] |> Async.Parallel
| OrderPlaced order ->
  let tabId = order.Tab.Id
  [
    actions.Table.ReceivedOrder tabId
    actions.Chef.AddFoodsToPrepare tabId order.Foods
    actions.Waiter.AddDrinksToServe tabId order.Drinks
  ] |> Async.Parallel
| DrinkServed (item, tabId) ->
  [actions.Waiter.MarkDrinkServed tabId item]
  |> Async.Parallel
| _ -> failwith "TODO"