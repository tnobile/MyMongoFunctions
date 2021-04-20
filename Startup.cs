using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoFunctions.Helpers;
using MongoDB.Driver;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.IO;
using MyMongoFunctions.Services;
using MyNotes.Functions;

[assembly: WebJobsStartup(typeof(Startup))]
namespace MyNotes.Functions
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
            builder.Services.AddSingleton<INoteService, NoteService>();
            builder.Services.AddSingleton((s) =>
            {
                var c = config[Settings.MONGO_CONNECTION_STRING];
                return new MongoClient(Environment.GetEnvironmentVariable(Settings.MONGO_CONNECTION_STRING));
            });

        }
    }
}