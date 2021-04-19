using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoFunctions.Helpers;
using MyNotes.Functions.Models;

namespace MyNotes.Functions
{
    public class CreateNote
    {

        private readonly MongoClient _mongoClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        private readonly IMongoCollection<Note> _notes;

        public CreateNote(
            MongoClient mongoClient,
            ILogger<GetNotes> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase(Environment.GetEnvironmentVariable(Settings.DATABASE_NAME));
            _notes = database.GetCollection<Note>(Environment.GetEnvironmentVariable(Settings.COLLECTION_NAME));
        }

        [FunctionName(nameof(CreateNote))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "note")] HttpRequest req,
            ILogger log)
        {
            IActionResult returnValue = null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<Note>(requestBody);

            var album = new Note
            {
                Word = input.Word,
                Category = input.Category,
                Content = input.Content,
            };

            try
            {
                await _notes.InsertOneAsync(album);
                returnValue = new OkObjectResult(album);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }


            return returnValue;
        }
    }
}