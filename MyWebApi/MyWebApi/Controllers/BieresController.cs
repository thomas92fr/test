using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;
using MyWebApi.Services;

namespace MyWebApi.Controllers
{
    /// <summary>
    /// Controleur chargé des opérations réalisables sur les bieres
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BieresController : ControllerBase
    {
        private readonly IBieresService _biereService;

        public BieresController(IBieresService biereService)
        {
            _biereService = biereService;
        }

        /// <summary>
        /// Retourne toutes les bieres 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllBieres()
        {
            IEnumerable<Biere> bières = await _biereService.GetAllBieresAsync();
            return Ok(bières);
        }

        /// <summary>
        /// Retourne les données de la biere portant l'id en parametre 
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBiereById(int id)
        {
            Biere? biere = await _biereService.GetBiereByIdAsync(id);
            if (biere == null)
            {
                return NotFound();
            }

            return Ok(biere);
        }

        /// <summary>
        /// Ajoute une biere en BDD
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddBiere([FromBody] Biere newBiere)
        {
            await _biereService.AddBiereAsync(newBiere);
            return CreatedAtAction(nameof(GetBiereById), new { id = newBiere.Id }, newBiere);
        }

        /// <summary>
        /// Modifie une biere en BDD
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBiere(int id, [FromBody] Biere updatedBiere)
        {
            if (id != updatedBiere.Id)
            {
                return BadRequest();
            }

            await _biereService.UpdateBiereAsync(updatedBiere);
            return NoContent();
        }

        /// <summary>
        /// Supprime une biere en BDD
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBiere(int id)
        {
            await _biereService.DeleteBiereAsync(id);
            return NoContent();
        }
    }
}
