using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;
using MyMongoFunctions.Services;

namespace MyNotes.Functions
{
    public class GetNote
    {
        private readonly ILogger<GetNote> _logger;
        private readonly INoteService _service;

        public GetNote(
            INoteService service,
            ILogger<GetNote> logger,
            IConfiguration config)
        {
            _service = service;
            _logger = logger;
        }

        [FunctionName(nameof(GetNote))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "note/{id}")] HttpRequest req, string id,
            ILogger log)
        {
            try
            {
                var result = await _service.GetNote(id);

                if (result == null)
                {
                    _logger.LogWarning("That item doesn't exist!");
                    return new NotFoundResult();
                }
                else
                {
                    return new OkObjectResult(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't find note with id: {id}. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
