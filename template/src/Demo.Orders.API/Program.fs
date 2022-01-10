module Demo.Orders.API.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open Giraffe

open Demo.Orders.API.Http

let configureCors (builder : CorsPolicyBuilder) =
    let allowedOrigins = [| "http://localhost:5000"; "https://localhost:5001" |]

    builder
        .WithOrigins(allowedOrigins)
        .AllowAnyMethod()
        .AllowAnyHeader ()
    |> ignore

let configureApp (app : IApplicationBuilder) =
    let env =
        let env = app.ApplicationServices.GetService<IWebHostEnvironment> ()

        match env.IsDevelopment () with
        | true -> app.UseDeveloperExceptionPage ()
        | false ->
            app
                .UseGiraffeErrorHandler(Root.errorHandler)
                .UseHttpsRedirection ()

    env
        .UseCors(configureCors)
        .UseGiraffe (Root.routes)

let configureServices (services : IServiceCollection) =
    services.AddCors () |> ignore
    services.AddGiraffe () |> ignore

let configureLogging (builder : ILoggingBuilder) = builder.AddConsole().AddDebug () |> ignore

[<EntryPoint>]
let main args =
    Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .Configure(Action<IApplicationBuilder> configureApp)
                .ConfigureServices(configureServices)
                .ConfigureLogging (configureLogging)
            |> ignore
        )
        .Build()
        .Run ()

    0
