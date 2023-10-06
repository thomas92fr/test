using MyWebApi.Models;

namespace MyWebApi.Services
{
    public interface IBrasserieService
    {
        public Task<IEnumerable<Brasserie>> GetAllBrasseriesAsync();

        Task<Brasserie?> GetBrasserieByIdAsync(int id);
    }
}
