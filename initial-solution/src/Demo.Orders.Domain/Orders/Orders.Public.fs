namespace Demo.Orders.Domain.Orders

[<CLIMutable>]
type UnvalidatedAddress =
    {
        street : string
        city : string
        state : string
        postalCode : string
    }

[<CLIMutable>]
type UnvalidatedOrderLine =
    {
        lineNumber : int
        quantity : int
        price : decimal
        sku : string
    }

[<CLIMutable>]
type UnvalidatedOrder =
    {
        orderId : string
        customerName : string
        orderLines : UnvalidatedOrderLine []
        shippingAddress : UnvalidatedAddress
    }
