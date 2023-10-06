using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;
using MyWebApi.Services;

namespace MyWebApi.Controllers
{
    /// <summary>
    /// Controleur chargé des opérations réalisables sur les brasserie
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BrasseriesController : ControllerBase
    {
        private readonly IBrasserieService _brasserieService;

        public BrasseriesController(IBrasserieService brasserieService)
        {
            _brasserieService = brasserieService;
        }

        /// <summary>
        /// Retourne toutes les brasseries 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var brasseries = await _brasserieService.GetAllBrasseriesAsync();
            return Ok(brasseries);
        }

        /// <summary>
        /// Retourne les données de la brasserie portant l'id en parametre 
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrasserieById(int id)
        {
            Brasserie? brasserie = await _brasserieService.GetBrasserieByIdAsync(id);
            if (brasserie == null)
            {
                return NotFound();
            }

            return Ok(brasserie);
        }

    }
}
