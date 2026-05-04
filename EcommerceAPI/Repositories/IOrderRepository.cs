using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(Order order);
    Task SaveChangesAsync();
}
