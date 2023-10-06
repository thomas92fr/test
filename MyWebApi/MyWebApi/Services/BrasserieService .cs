using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;

namespace MyWebApi.Services
{
    /// <summary>
    /// Service chargée des opération sur les données des brasseries
    /// </summary>
    public class BrasserieService: IBrasserieService
    {
        private readonly AppDbContext _context;

        public BrasserieService(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Retourne la liste de toutes les brasseries
        /// </summary>
        public async Task<IEnumerable<Brasserie>> GetAllBrasseriesAsync()
        {
            return await _context.Brasseries
                .Include(br => br.Bieres)
                .ThenInclude(b => b.GrossisteBieres)
                .ToListAsync();
        }

        /// <summary>
        /// Retourne les données de la brasserie portant l'id en parametre 
        /// </summary>
        public async Task<Brasserie?> GetBrasserieByIdAsync(int id)
        {
            return await _context.Brasseries
                .Include(br => br.Bieres)
                .ThenInclude(b => b.GrossisteBieres)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
 