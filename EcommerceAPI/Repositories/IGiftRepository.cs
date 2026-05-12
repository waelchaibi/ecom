using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface IGiftRepository
{
    Task<List<Gift>> GetAllAsync();
    Task<Gift?> GetByIdAsync(int id);
    Task<Gift> AddAsync(Gift gift);
    Task SaveChangesAsync();
}
