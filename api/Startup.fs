module Startup

open Microsoft.Azure.Functions.Extensions.DependencyInjection
open Microsoft.Extensions.DependencyInjection
open Microsoft.Azure.WebJobs.Extensions.CosmosDB
open Bindings
open Microsoft.Extensions.Configuration
open Microsoft.Azure.Cosmos
open System

type Startup() =
    inherit FunctionsStartup()
    let config = (new ConfigurationBuilder())
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile("local.settings.json", true)
                    .AddEnvironmentVariables()
                    .Build();
    override this.ConfigureAppConfiguration(builder: IFunctionsConfigurationBuilder) =
        ()
    override this.Configure(builder: IFunctionsHostBuilder) =
        builder.Services.AddLogging() |> ignore
        builder.Services.AddSingleton<IConfiguration, IConfiguration>(fun _ -> config) |> ignore
        builder.Services.AddSingleton<ICosmosDBSerializerFactory, ThothCosmosDbSerializerFactory>()
            |> ignore
        builder.Services.AddSingleton<CosmosClient, CosmosClient>(
            fun svc ->
                let cosmosConnectionString = config["CosmosDbConnectionString"]
                if cosmosConnectionString = null then
                    failwith "You must set CosmosDbConnectionString in app settings"
                else
                    let factory: ICosmosDBSerializerFactory = ThothCosmosDbSerializerFactory()
                    new CosmosClient(
                        cosmosConnectionString,
                        CosmosClientOptions(
                            Serializer = factory.CreateSerializer()))
            ) |> ignore

[<assembly: FunctionsStartup(typeof<Startup>)>]
do ()