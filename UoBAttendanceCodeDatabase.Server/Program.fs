module UoBAttendanceCodeDatabase.Server.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Microsoft.AspNetCore.Http

let indexHandler : HttpHandler =
    fun next ctx ->
        task {
            let db = ctx.GetService<Database.AttendanceCodeContext>()
            return! htmlView (Views.Index.index { Codes = db.AttendanceCodes |> Seq.toList }) next ctx
        }


let addCodeHandler : HttpHandler =
    fun next ctx ->
        task {
            let! code = ctx.BindFormAsync<Models.AttendanceCodes.AttendanceCode>()
            if code.Code = 0 || String.IsNullOrWhiteSpace code.Subject || code.DateAdded = DateTime.MinValue then
                return! (setStatusCode 400 >=> text "Invalid input. `code`, `subject`, and `dateAdded` are required.") next ctx
            else
                let db = ctx.GetService<Database.AttendanceCodeContext>()
                db.AttendanceCodes.Add(code) |> ignore
                do! db.SaveChangesAsync() |> Async.AwaitTask |> Async.Ignore
                return! redirectTo false "/" next ctx
        }


let webApp =
    choose [
        GET >=> choose [
            route   "/" >=> indexHandler 
        ]
        POST >=> choose [
            route "/add-code" >=> addCodeHandler
        ]
        setStatusCode 404 >=> text "Not Found"
    ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse
        >=> setStatusCode 500 
        >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder: CorsPolicyBuilder) =
    builder
        .WithOrigins("http://localhost:5000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        |> ignore

let configureApp (app: IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    let app = match env.IsDevelopment() with
                | true  -> app.UseDeveloperExceptionPage()
                | false -> app.UseGiraffeErrorHandler(errorHandler).UseHttpsRedirection()
    
    app .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services: IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore
    services.AddDbContext<Database.AttendanceCodeContext>() |> ignore

let configureLogging (builder: ILoggingBuilder) =
    builder.AddConsole()
    #if DEBUG
           .AddDebug() |> ignore
    #endif

[<EntryPoint>]
let main args =
    Database.initializeDatabase()
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureServices(configureServices)
                    .ConfigureLogging(configureLogging)
                    |> ignore
        )
        .Build()
        .Run()
    0