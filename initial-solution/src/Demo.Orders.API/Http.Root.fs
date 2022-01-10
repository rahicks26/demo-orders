module Demo.Orders.API.Http.Root

open System
open Microsoft.Extensions.Logging

open Giraffe

let private apiRoutes : HttpHandler = choose []

/// The routes that our api exposes
let routes : HttpHandler =
    choose [ subRoute "/api" apiRoutes; setStatusCode 404 >=> text "Not Found" ]

/// The root level error handler
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError (ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message
