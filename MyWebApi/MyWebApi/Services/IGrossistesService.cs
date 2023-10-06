using MyWebApi.Models;

namespace MyWebApi.Services
{
    public interface IGrossistesService
    {
        Task<IEnumerable<Grossiste>> GetAllGrossistesAsync();
        Task<Grossiste?> GetGrossisteByIdAsync(int id);

        Task<Devis> DemanderDevisAsync(int grossisteId, List<DevisLigne> commande);

        Task MajGrossisteBiereQuantiteEnStockAsync(int grossisteId, int biereId, int quantiteEnStock);
        Task AddGrossisteAsync(Grossiste newGrossiste);
        Task UpdateGrossisteAsync(Grossiste updatedGrossiste);
        Task DeleteGrossisteAsync(int id);
    }
}
