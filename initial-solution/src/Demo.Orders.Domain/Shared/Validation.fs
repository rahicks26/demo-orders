module Demo.Orders.Domain.Shared.Validation

open System
open System.Text.RegularExpressions

let isStringOfLength len cstor (name:string)  =
    function
    | str when str |> String.IsNullOrEmpty -> Error $"{name} cannot be empty."
    | str when str.Length > len -> Error $"{name} cannot be longer than {len}."
    | str -> str |> cstor |> Ok

let stringMatches pattern cstor (name:string)  =
    function
    | str when str |> String.IsNullOrEmpty -> Error $"{name} cannot be empty."
    | str when Regex.IsMatch (str, pattern) |> not -> Error $"{name} does not conform to {pattern}."
    | str -> str |> cstor |> Ok

let isAbove limit cstor (name:string)  =
    function
    | num when num < limit -> Error $"{name} must be strictly above {limit}"
    | num -> num |> cstor |> Ok