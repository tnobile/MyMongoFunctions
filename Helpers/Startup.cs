using Microsoft.Azure.WebJobs;
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

[assembly: WebJobsStartup(typeof(Startup))]
namespace MyNotes.Functions.Helpers
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
             builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            builder.Services.Configure<IConfiguration>(c=>{
                var b =c;
            });
            var config = (IConfiguration)builder.Services.First(d => d.ServiceType == typeof(IConfiguration)).ImplementationInstance;

            builder.Services.AddSingleton((s) =>
            {
                var c = config;
                return new MongoClient(Environment.GetEnvironmentVariable(Settings.MONGO_CONNECTION_STRING));
            });
        }
    }
}