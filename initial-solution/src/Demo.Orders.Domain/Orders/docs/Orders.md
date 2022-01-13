# Orders

The order model has quickly taken shape and is mainly based off a call with the team at Demo tech. We will use our docs folder here to capture this call and any follow ups, so the development team always has it close at hand.


## Discovery Call Notes
We will be keeping our order model very simple here at Demo tech. That is the way we like things. When a customer places an order it will be for one of our many products that we always track by sku. They will ask for an amount and be locked into a price per amount. The order will be shipped to them directly. 

So how do you look up orders? 

We always use our order id. We have some rules around how we make them in our ERP tool, but you will have to meet with the Jim to get the details on that.

What about order lines?

Hmmm we have a count on there, I think it is the Order Line Number, but check with Jim.

### Drafted Model

```
let type Order =
  OrderId: OrderId
  CustomerName: FullName
  OrderLines: OrderLine list
  ShippingAddress: Address

let type OrderLine =
  OrderLineNumber: Count
  Price: Money
  Quantity: Count
  Sku: Sku

let type Address =
  Street: Text
  City: Text
  State: Text
  PostalCode: Text
```