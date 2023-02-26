module Startup

open Microsoft.Azure.Functions.Extensions.DependencyInjection
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open System
open System.Data.SqlClient

type Startup() =
    inherit FunctionsStartup()
    let config = (new ConfigurationBuilder())
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile("local.settings.json", true)
                    .AddEnvironmentVariables()
#if DEBUG
                    .AddJsonFile("secret.settings.json", true)
#endif
                    .Build();
    override this.ConfigureAppConfiguration(builder: IFunctionsConfigurationBuilder) =
        ()
    override this.Configure(builder: IFunctionsHostBuilder) =
        builder.Services.AddLogging()
            .AddScoped<SqlConnection>(fun _ -> new SqlConnection (config["SQLConnectionString"]))
            |> ignore

[<assembly: FunctionsStartup(typeof<Startup>)>]
do ()