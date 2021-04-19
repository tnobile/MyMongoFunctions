using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;
using MongoFunctions.Helpers;
using MyNotes.Functions.Models;

namespace MyNotes.Functions
{
    public class GetNotes
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Note> _notes;

        public GetNotes(
            MongoClient mongoClient,
            ILogger<GetNotes> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase(config[Settings.DATABASE_NAME]);
            _notes = database.GetCollection<Note>(config[Settings.COLLECTION_NAME]);
        }

        [FunctionName(nameof(GetNotes))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "note")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult(await _notes.Find(f => true).ToListAsync());
        }
    }
}
