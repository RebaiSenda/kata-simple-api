using KataSimpleAPI.Dtos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace KataSimpleAPI.Controllers
{
    [Route("api/data")]
    [ApiController]

    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;

        public DataController(ILogger<DataController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize("KataSimpleAPI")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SimpleDataResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<SimpleDataResponse> GetData()
        {
            _logger.LogInformation("GetData endpoint called in KataSimpleAPI");

            // Données statiques à retourner en dur
            var data = new SimpleDataResponse
            {
                Message = "Données complémentaires pour le système de réservation",
                Items =
               [
                   new() { Id = 1, Name = "Informations importantes" },
                    new() { Id = 2, Name = "Règles de réservation" },
                    new() { Id = 3, Name = "Consignes spéciales" },
                    new() { Id = 4, Name = "Contacts administratifs" },
                    new() { Id = 5, Name = "Procédures d'urgence" }
               ]
            };

            return Ok(data);
        }
    }
}

