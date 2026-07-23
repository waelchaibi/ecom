using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(int? categoryId = null, bool activeOnly = true);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<bool> HasOrderItemsAsync(int productId);
    Task RemoveFromAllCartsAsync(int productId);
    Task SaveChangesAsync();

    /// <summary>Atomically reduces stock if enough is available. Returns false if product missing or insufficient stock.</summary>
    Task<bool> TryDecrementStockAsync(int productId, int quantity, DateTime updatedAtUtc, CancellationToken cancellationToken = default);

    Task IncrementStockAsync(int productId, int quantity, DateTime updatedAtUtc, CancellationToken cancellationToken = default);
}
