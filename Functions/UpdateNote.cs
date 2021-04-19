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
using Newtonsoft.Json;
using System.IO;

namespace MyNotes.Functions
{
    public class UpdateNote 
    {
        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Note> _notes;

        public UpdateNote(
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

        [FunctionName(nameof(UpdateNote))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/{id}")] HttpRequest req, string id,
            ILogger log)
        {
           IActionResult returnValue = null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var updatedResult = JsonConvert.DeserializeObject<Note>(requestBody);

            updatedResult.Id = id;

            try
            {
                var replacedItem = await _notes.ReplaceOneAsync(album => album.Id == id, updatedResult);

                if (replacedItem == null)
                {
                    returnValue = new NotFoundResult();
                }
                else
                {
                    returnValue = new OkObjectResult(updatedResult);
                }              
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not update note with id: {id}. Exception thrown: {ex.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return returnValue;
        }
    }
}