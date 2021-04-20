using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using MyMongoFunctions.Services;
using System.Linq;

namespace MyNotes.Functions
{
    public class GetNotes
    {
        private readonly ILogger<GetNotes> _logger;
        private readonly INoteService _service;

        public GetNotes(
            INoteService service,
            ILogger<GetNotes> logger,
            IConfiguration config)
        {
            _service = service;
            _logger = logger;
        }

        [FunctionName(nameof(GetNotes))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "note")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var books = await _service.GetNotes();

                if (!books.Any())
                {
                    _logger.LogWarning("No notes found!");
                    return new StatusCodeResult(StatusCodes.Status404NotFound);
                }

                return new OkObjectResult(books);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}