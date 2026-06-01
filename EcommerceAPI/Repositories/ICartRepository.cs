using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByCustomerIdAsync(int customerId);
    Task<Cart> GetOrCreateAsync(int customerId);
    Task SaveChangesAsync();
    Task ClearItemsAsync(int cartId);
}
