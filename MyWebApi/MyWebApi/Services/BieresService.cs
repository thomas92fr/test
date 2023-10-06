using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;

namespace MyWebApi.Services
{
   /// <summary>
   /// Service chargée des opération sur les données des bieres
   /// </summary>
    public class BieresService : IBieresService
    {
        private readonly AppDbContext _context;

        public BieresService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retourne la liste de toutes les bieres
        /// </summary>
        public async Task<IEnumerable<Biere>> GetAllBieresAsync()
        {
            return await _context.Bieres
                .Include(b => b.Brasserie)
                .Include(b => b.GrossisteBieres)
                .ToListAsync();
        }

        /// <summary>
        /// Retourne les données de la biere portant l'id en parametre 
        /// </summary>
        public async Task<Biere?> GetBiereByIdAsync(int id)
        {
            return await _context.Bieres
                .Include(b => b.Brasserie)
                .Include(b => b.GrossisteBieres)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <summary>
        /// Ajoute une biere en BDD
        /// </summary>
        public async Task AddBiereAsync(Biere biere)
        {
            await _context.Bieres.AddAsync(biere);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Modifie une biere en BDD
        /// </summary>
        public async Task UpdateBiereAsync(Biere biere)
        {
            _context.Bieres.Update(biere);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Supprime une biere en BDD
        /// </summary>
        public async Task DeleteBiereAsync(int id)
        {
            var biere = await GetBiereByIdAsync(id);
            if (biere != null)
            {
                _context.Bieres.Remove(biere);
                await _context.SaveChangesAsync();
            }
        }
    }
}
