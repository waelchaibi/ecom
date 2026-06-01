using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface IGiftRepository
{
    Task<List<Gift>> GetAllAsync();
    Task<Gift?> GetByIdAsync(int id);
    Task<Gift> AddAsync(Gift gift);
    Task SaveChangesAsync();

    /// <summary>Atomically reduces gift stock when at least <paramref name="quantity"/> is in stock.</summary>
    Task<bool> TryDecrementStockAsync(int giftId, int quantity, CancellationToken cancellationToken = default);

    Task IncrementStockAsync(int giftId, int quantity, CancellationToken cancellationToken = default);
}
