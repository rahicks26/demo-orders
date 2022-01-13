module Demo.Orders.Domain.Orders.Primitives

open System

open FsToolkit.ErrorHandling

open Demo.Orders.Domain.Shared
open Demo.Orders.Domain.Shared.Primitives

type Sku = private Sku of string
type Price = private Price of decimal
type OrderId = private OrderId of string
type OrderLineNumber = private OrderLineNumber of int
type FullName = private FullName of string

[<Struct>]
type Address =
    {
        Street : String100
        City : String50
        State : String50
        PostalCode : PostalCode
    }

and PostalCode = private PostalCode of string

module Sku =
    let create = Validation.stringMatches "[^A-Za-z0-9]" Sku "Sku" 

    let value (Sku str) = str

module Price =
    let create = Validation.isAbove 0m  Price "Price"

    let value (Price m) = m

module OrderId =
    let create = Validation.isStringOfLength 50 OrderId "Order Id"

    let value (OrderId str) = str

    let newOrderId orderIdFactory =
        orderIdFactory() |> String50.value |> OrderId

    let guidOrderFactory (newGuid: unit -> Guid) =
        newGuid
        >> string
        >> String50.create "Order Id"
        >> Result.valueOr (fun err -> failwith $"Unexpected error when generating a guid. Error: {err}" )

module OrderLineNumber =
    let create = Validation.isAbove 0  OrderLineNumber "Order Line Number"

    let value (OrderLineNumber num) = num

module FullName =
    let create = Validation.isStringOfLength 100  FullName "Full Name"

    let value (FullName str) = str

module Address =
    let create dto =
        validation {
            let! street = dto.street |> String100.create "Address Street" 
            and! city = dto.city |> String50.create "Address City" 
            and! state = dto.state |> String50.create "Address State" 
            and! postalCode = dto.postalCode |> Validation.isStringOfLength 5 PostalCode "Address Postal Code"

            return
                {
                    Street = street
                    City = city
                    State = state
                    PostalCode = postalCode
                }
        }

    let toUnvalidated model =
        let street = model.Street |> String100.value
        let city = model.City |> String50.value
        let state = model.State |> String50.value
        let (PostalCode postalCode) = model.PostalCode

        {
            street = street
            city = city
            state = state
            postalCode = postalCode
        }
