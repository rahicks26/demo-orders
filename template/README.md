# Template 
There is a bit of setup required that can be a struggle in F#. So we will define a simple template to help.  We may set this up so that it will be scaffolded with `dotnet new`, but that is out of scope for this talk.

 It can be hard to know where to start or how to organize your code. This may stem from the fact that with code there are so many right ways. We will make life easier and start with a test-heavy solution. This is mainly because we will be focusing on testing in this talk, but this will help us keep our tests focused and organized.

```
_ src
|__ {Name}.API
|__ {Name}.Domain
_ tests
|__ {Name}.API.Integration.Tests
|__ {Name}.API.Unit.Tests
|__ {Name}.Domain.Unit.Tests
|__ {Name}.E2E.Tests
|__ {Name}.Public.Contract.Tests
```

We will also be using the following libraries:

- Fantomas
- Expecto
- Giraffe
- Paket

## How to do this yourself ;)
I get it your a dev and you know how to use `dotnet new` so why not just do it yourself. Well here are the steps we used to build this template. If for some reason they don't quite work for you then be sure to let us know. 

Be sure to replace `{Name}`  with the name of your solution. When you get moving with posts like this it is easy to forget ;)

### Setting up the solution
We can start by building out an empty solution.
```
dotnet new gitignore
dotnet new sln -n {Name}
```
### Adding Helpful templates
We will be using the following templates to speed things up a bit. Just in case you don't have them this is how you can install them.
```
dotnet new -i Expecto.Template 
dotnet new -i "giraffe-template::*"
```
### Adding projects for the solution
Now, let's build out our projects and add them to the solution.
```
dotnet new giraffe -V none -n {Name}.API -o src/{Name}.API
dotnet new classlib -lang F# -n {Name}.Domain -o src/{Name}.Domain
dotnet new expecto -n {Name}.API.Unit.Tests -o tests/{Name}.API.Unit.Tests
dotnet new expecto -n {Name}.Domain.Unit.Tests -o tests/{Name}.Domain.Unit.Tests
dotnet new expecto -n {Name}.API.Integration.Tests -o tests/{Name}.API.Integration.Tests
dotnet sln add ./**/*.fsproj    
```
To keep things simple we will update all the projects to use the same version of `dotnet`. Our class library should have been built with the default for your env. So copy your `target-framework` from the it's `.fsproj` file, which should look like `<TargetFramework>net6.0</TargetFramework>`. You can use it to update the rest of your projects. 

### Setting up Paket
We will be using Paket for managing our packages. Paket had great tooling in place in place, so getting it set up won't be hard at all. 
```
dotnet restore
dotnet tool install paket --version 7.0.0-alpha003
dotnet paket convert-from-nuget --no-install
```
Next, we will make a small change to `paket.dependencies`. Open it up and set the storage to symlink like so: `storage: symlink`. 
```
dotnet paket install
dotnet paket simplify
```
Finally,  we will do is update our `.gitignore` to ignore `.paket` file.

### Setting up Formatting
We want to install the `fantomas-tool` to make code formatting easier. While there are plenty of plugins and extensions to leverage, it is always nice to have the same tool available to you that is ideal for your build pipeline.
```
dotnet tool install fantomas-tool --version 4.6.0-beta-001   
wget https://raw.githubusercontent.com/G-Research/fsharp-formatting-conventions/master/.editorconfig
```
This is a good time to update this config to use a bit more width. We generally update our config to set most line limits to 80 and max to 120, so that it works nicely with ultra-wide screens.

```
root = true
indent_size=4
max_line_length=120

[*.fs]
fsharp_space_before_uppercase_invocation=true
fsharp_space_before_member=true
fsharp_space_before_colon=true
fsharp_multiline_block_brackets_on_same_column=true
fsharp_newline_between_type_definition_and_members=true
fsharp_keep_if_then_in_same_line=true
fsharp_align_function_signature_to_indentation=true
fsharp_alternative_long_member_definitions=true
fsharp_disable_elmish_syntax=true
fsharp_multi_line_lambda_closing_newline=true

# Customizations on top of G-Research
fsharp_max_value_binding_width=80
fsharp_max_function_binding_width=80
fsharp_max_if_then_else_short_width=60
fsharp_max_infix_operator_expression=80
fsharp_max_record_width=80
fsharp_max_array_or_list_width=80
```

With this in place, you can format your code by running `dotnet fantomas -r ./`.

### Final setup up.
To wrap things up we have a bit of housekeeping to do. 

We need to remove the files that we won't need from scaffolding out our projects. 

- Remove `Sample.fs` from each test project
- Remove the sample files in `{Name}.Domain`

We also want to add a reference to `{Name}.Domain` in`{Name}.API`.
```
dotnet add ./src/Demo.Orders.API/Demo.Orders.API.fsproj reference  ./src/Demo.Orders.Domain 
```
Finally, we can clean up the API file a bit now. We will start by deleting:

- **HttpHandler.fs** while this is a great example we don't need it in a template
- **Model.fs** we will handle things like this a tad bit different as you will see

And finish up by splitting the `Program.fs` file into two files. One to handle HTTP setup and the other for app setup.

Let update `Program.fs` to look like:

```
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
```
Then we add `Http.Root.fs` to the `{Name}.API` and set it to:
```
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
arResponse >=> setStatusCode 500 >=> text ex.Message

```

## Resources for this template
We covered a lot and if you are interested in knowing more you can find considerably more information below.

- [Expecto Template](https://github.com/haf/expecto#net-core-support)
- [Giraffe Template](https://github.com/giraffe-fsharp/Giraffe#using-dotnet-new)
- [Fantomas](https://github.com/fsprojects/fantomas/blob/master/docs/Documentation.md#using-the-command-line-tool)
- [G-Research Formatting Style](https://github.com/G-Research/fsharp-formatting-conventions)
- [Expecto](https://github.com/haf/expecto)
- [Giraffe](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md)
- [Paket](https://fsprojects.github.io/Paket/get-started.html)
- [Dotnet CLI introduction](https://docs.microsoft.com/en-us/dotnet/core/tools)