using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IPDFService
    {
        Task<IEnumerable<PDF>> GetAllAsync();
        Task<PDF?> GetByIdAsync(int id);
        Task<PDF?> GetByFileNameAsync(string? filename);
        Task AddAsync(PDF pdf);
        Task UpdateAsync(PDF pdf);
        Task DeleteAsync(int id);

    }
}