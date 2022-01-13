namespace Demo.Orders.Domain.Orders

open System

open System.Threading.Tasks



open Demo.Orders.Domain.Orders.Primitives
open Demo.Orders.Domain.Shared.Primitives
open Demo.Orders.Domain.Shared

type OrderHeader =
    {
        CustomerName : FullName
        OrderLines : OrderLine list
        ShippingAddress : Address
    }

and OrderLine =
    {
        LineNumber : OrderLineNumber
        Quantity : PositiveInt
        Price : Price
        Sku : Sku
        ProductName: String100
    }

type Order =
| OpenOrder of OrderHeader * OrderId 
| ClosedOrder of OrderHeader * OrderId * DateTimeOffset

[<AutoOpen>]
module OrderAgg =

    type CreateOrder = 
        {
            CustomerName: FullName
            ShippingAddress: Address
        }

    type AddOrderLine = 
        {
            Quantity: PositiveInt
            Price: Price
            Sku: Sku
            ProductName: String100
        }

    type RemoveOrderLine = 
        {
            LineNumber: OrderLineNumber
        }

    type CompleteOrder = 
        {
            CompletedAt: DateTimeOffset
        }

    type OrderCommannd =
        | CreateOrder of CreateOrder
        | AddOrderLine of AddOrderLine
        | RemoveOrderLine of RemoveOrderLine
        | CompleteOrder of CompleteOrder

    type OrderEvent =
        | OrderCreated of OrderHeader * OrderId
        | OrderLineAdded of OrderLine 
        | OrderLineRemoved of OrderLineNumber
        | OrderCompleted of DateTimeOffset

    let upsertHeader newOrderId cmd: Order * OrderId =
        function
        | Some (header, orderId) ->
            let header = 
                {header with CustomerName = cmd.CustomerName; ShippingAddress = cmd.ShippingAddress }
            (header,orderId) |> OpenOrder
        | None ->
            let header =
                {
                    CustomerName = cmd.CustomerName
                    OrderLines = []
                    ShippingAddress = cmd.ShippingAddress
                }
            let orderId = newOrderId()

            (header, orderId) |> OpenOrder    


   

    let private decide cmd state =
        match state, cmd with
        | Some (ClosedOrder _) , _ -> Error "Cannot preform any commands on closed orders"
        | Some (OpenOrder (header, orderId)), AddOrderLine cmd -> OrderEvent.Placeholder |> List.singleton |> Ok
        | Some (OpenOrder (header, orderId)), RemoveOrderLine cmd ->OrderEvent.Placeholder |> List.singleton |> Ok
        | Some (OpenOrder (header, orderId)), CompleteOrder cmd ->OrderEvent.Placeholder |> List.singleton |> Ok
        | Some (OpenOrder _), CreateOrder _ -> Error "We cannot create an order header for an existing order"
        | None, CreateOrder cmd -> OrderEvent.Placeholder |> List.singleton |> Ok
        | None, _ -> Error "Please create the order header first"

    let private isTerminal =
        function 
        | Some (ClosedOrder _) -> true 
        | _ -> false

    let private evolve state event =
        let appendOrderLine header line = 
            {header with OrderLines = line::header.OrderLines}

        let removeOrderLine header line = 
            {header with OrderLines = header.OrderLines |> List.filter(fun l -> l = line)}

        match state, event with
        | Some (ClosedOrder (header,orderId,closedOn)), _  -> (header,orderId,closedOn) |> ClosedOrder|> Some
        | None, OrderCreated (order, orderId) -> (order, orderId) |> OpenOrder |> Some
        | Some _, OrderCreated (order, orderId) -> (order, orderId) |> OpenOrder |> Some
        | None,  OrderLineAdded (line,header, orderId) -> 
            (line |> appendOrderLine header, orderId) |> OpenOrder |> Some 
        | Some (OpenOrder (header, orderId) ), OrderLineAdded (line) -> 
            (line |> appendOrderLine header, orderId) |> OpenOrder |> Some 
        | None,  OrderLineRemoved (line,header, orderId) -> 
            (line |> removeOrderLine header, orderId) |> OpenOrder |> Some 
        | Some (OpenOrder (header, orderId) ), OrderLineRemoved (line) -> 
            (line |> removeOrderLine header, orderId) |> OpenOrder |> Some 
        | None, OrderCompleted(closedAt) ->
            (header, orderId, closedAt) |> ClosedOrder |> Some
        | Some (OpenOrder (header, orderId) ), OrderCompleted(closedAt) -> 
            (header, orderId, closedAt) |> ClosedOrder |> Some

    let decider = 
        {
            decide = decide
            evolve = evolve
            initialState = None
            isTerminal = isTerminal
        }