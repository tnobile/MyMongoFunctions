using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using MyNotes.Functions.Models;
using MyMongoFunctions.Services;

namespace MyNotes.Functions
{
    public class CreateNote
    {
        private readonly ILogger<CreateNote> _logger;
        private readonly INoteService _service;

        public CreateNote(
            INoteService service,
            ILogger<CreateNote> logger,
            IConfiguration config)
        {
            _service = service;
            _logger = logger;
        }

        [FunctionName(nameof(CreateNote))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "note")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<Note>(requestBody);

            var note= new Note
            {
                Word = input.Word,
                Category = input.Category,
                Content = input.Content,
            };

            try
            {
                await _service.CreateNote(note);
                return new OkObjectResult(note);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}