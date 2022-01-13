module Demo.Orders.Domain.Shared.Primitives

type String50 = private String50 of string
type String100 = private String100 of string
type PositiveInt = private PositiveInt of int

module PositiveInt =
    let create = Validation.isAbove 0 PositiveInt

    let value (PositiveInt num) = num

module String50 =
    let create = Validation.isStringOfLength 50 String50

    let value (String50 str) = str

module String100 =
    let create = Validation.isStringOfLength 100 String100

    let value (String100 str) = str