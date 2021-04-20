using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using MyNotes.Functions.Models;
using Newtonsoft.Json;
using System.IO;
using MyMongoFunctions.Services;

namespace MyNotes.Functions
{
    public class UpdateNote
    {
        private readonly ILogger<UpdateNote> _logger;
        private readonly INoteService _service;

        public UpdateNote(
            INoteService service,
            ILogger<UpdateNote> logger,
            IConfiguration config)
        {
            _service = service;
            _logger = logger;
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
                var result= await _service.UpdateBook(id, updatedResult);

                if (result.ModifiedCount!=1)
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