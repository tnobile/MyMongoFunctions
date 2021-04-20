using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using MyMongoFunctions.Services;

namespace MyNotes.Functions
{
    public class DeleteNote
    {
        private readonly ILogger<DeleteNote> _logger;
        private readonly INoteService _service;

        public DeleteNote(
            INoteService service,
            ILogger<DeleteNote> logger,
            IConfiguration config)
        {
            _service = service;
            _logger = logger;
        }

        [FunctionName(nameof(DeleteNote))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "note/{id}")] HttpRequest req, string id,
            ILogger log)
        {
            try
            {
                var result = await _service.RemoveNoteById(id);

                if (result.DeletedCount!=1)
                {
                    _logger.LogInformation($"Album with id: {id} does not exist. Delete failed");
                    return new StatusCodeResult(StatusCodes.Status404NotFound);
                }

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't find note with id: {id}. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
