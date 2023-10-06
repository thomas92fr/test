using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;
using MyWebApi.Services;

namespace MyWebApi.Controllers
{
    /// <summary>
    /// Controleur chargé des opérations réalisables sur les grossistes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GrossistesController : ControllerBase
    {
        private readonly IGrossistesService _grossistesService;

        public GrossistesController(IGrossistesService grossistesService)
        {
            _grossistesService = grossistesService;
        }

        /// <summary>
        /// Retourne tous les grossistes 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllGrossistes()
        {
            IEnumerable<Grossiste> grossistes = await _grossistesService.GetAllGrossistesAsync();
            return Ok(grossistes);
        }

        /// <summary>
        /// Retourne les données du grossiste portant l'id en parametre 
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrossisteById(int id)
        {
            Grossiste? biere = await _grossistesService.GetGrossisteByIdAsync(id);
            if (biere == null)
            {
                return NotFound();
            }

            return Ok(biere);
        }

        /// <summary>
        /// Calcul le montant d'une commande de bieres pour un grossiste
        /// </summary>
        /// <param name="grossisteId">Identifiant unique du grossiste concerné par le devis</param>
        /// <param name="commande">Liste des bieres commandées avec la quantité</param>
        [HttpPost("{id}/devis")]
        public async Task<IActionResult> DemanderDevis(int grossisteId, [FromBody] List<DevisLigne> commande)
        {
            try
            {
                Devis? result = await _grossistesService.DemanderDevisAsync(grossisteId, commande);
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Permet de mettre en vente une quantité d'une biere chez un grossiste, ou de modifier la quantité en stock si la biere est déja en stock
        /// </summary>
        /// <param name="grossisteId">Identifiant unique du grossiste concerné</param>
        /// <param name="biereId">Identifiant unique de la biere concernée</param>
        /// <param name="quantiteEnStock">Quantité en stock a créer/modifier</param>
        [HttpPut("majStock")]
        public async Task<IActionResult> MajGrossisteBiereQuantiteEnStock(int grossisteId, int biereId, int quantiteEnStock)
        {
            try
            {
                await _grossistesService.MajGrossisteBiereQuantiteEnStockAsync(grossisteId, biereId, quantiteEnStock);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Ajout d'un grossiste dans la BDD
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddGrossiste([FromBody] Grossiste newGrossiste)
        {
            await _grossistesService.AddGrossisteAsync(newGrossiste);
            return CreatedAtAction(nameof(GetGrossisteById), new { id = newGrossiste.Id }, newGrossiste);
        }

        /// <summary>
        /// Modification d'un grossiste dans la BDD
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrossiste(int id, [FromBody] Grossiste updatedGrossiste)
        {
            if (id != updatedGrossiste.Id)
            {
                return BadRequest();
            }

            await _grossistesService.UpdateGrossisteAsync(updatedGrossiste);
            return NoContent();
        }

        /// <summary>
        /// Suppresion d'un grossiste dans la BDD
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrossiste(int id)
        {           
            await _grossistesService.DeleteGrossisteAsync(id);
            return NoContent();
        }
    }
}
