using MyWebApi.Models;

namespace MyWebApi.Services
{
    public interface IBieresService
    {
        Task<IEnumerable<Biere>> GetAllBieresAsync();
        Task<Biere?> GetBiereByIdAsync(int id);
        Task AddBiereAsync(Biere biere);
        Task UpdateBiereAsync(Biere biere);
        Task DeleteBiereAsync(int id);
    }

}
