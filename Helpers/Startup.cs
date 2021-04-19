using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoFunctions.Helpers;
using MongoDB.Driver;
using System;
using MyNotes.Functions.Helpers;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.IO;

[assembly: WebJobsStartup(typeof(Startup))]
namespace MyNotes.Functions.Helpers
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
              var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            builder.Services.AddSingleton((s) =>
            {
                var c = config[Settings.MONGO_CONNECTION_STRING];
                return new MongoClient(Environment.GetEnvironmentVariable(Settings.MONGO_CONNECTION_STRING));
            });
        }
    }
}