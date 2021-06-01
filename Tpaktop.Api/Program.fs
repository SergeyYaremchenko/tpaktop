module Tpaktop.Api

open System

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe

open Serilog
open Serilog.Events

open CreateTask.Model
open CreateTask.Handler

let webApp =
    choose [
        subRouteCi "/task"
            (choose [
                POST >=> choose [
                    route "" >=> bindJson<CreateTaskModelUnvalidated> createTaskHandler
                ]
        ])
    
    
        // If none of the routes matched then return a 404
        RequestErrors.NOT_FOUND "Not Found"
    ]

open Microsoft.Extensions.Logging
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

//TODO: read logging config from IConfiguration
Log.Logger <-
    LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        //.WriteTo.File(JsonFormatter(), "log-{Date}.log")
        .CreateLogger()

let configureCors (builder : CorsPolicyBuilder) =
    //TODO: read origins from IConfiguration
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (app : IApplicationBuilder) =    
    let env = app.ApplicationServices.GetService<IHostEnvironment>()
    let builder = 
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage()
        else
            app.UseGiraffeErrorHandler errorHandler
    
    builder.UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Error
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main args =
        try
            Log.Information "Tpaktop starting up..."
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    fun webHostBuilder ->
                        webHostBuilder
                            .Configure(configureApp)
                            .ConfigureServices(configureServices)
                            .ConfigureLogging(configureLogging)
                            |> ignore)
                .UseSerilog()
                .Build()
                .Run()
            Log.Information "Tpaktop shutting down..."
            0
        with ex -> 
            Log.Fatal("Host terminated unexpectedly.", ex)
            1