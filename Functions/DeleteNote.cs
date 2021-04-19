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
    public class DeleteNote
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Note> _notes;

        public DeleteNote(
            MongoClient mongoClient,
            ILogger<GetNote> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase(Environment.GetEnvironmentVariable(Settings.DATABASE_NAME));
            _notes = database.GetCollection<Note>(Environment.GetEnvironmentVariable(Settings.COLLECTION_NAME));
        }

        [FunctionName(nameof(DeleteNote))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "note/{id}")] HttpRequest req, string id,
            ILogger log)
        {
            IActionResult returnValue = null;

            try
            {
                var result = await _notes.DeleteManyAsync(n => n.Id == id);

                if (result == null)
                {
                    _logger.LogInformation($"Album with id: {id} does not exist. Delete failed");
                    returnValue = new StatusCodeResult(StatusCodes.Status404NotFound);
                }

                returnValue = new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't find note with id: {id}. Exception thrown: {ex.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return returnValue;
        }
    }
}
