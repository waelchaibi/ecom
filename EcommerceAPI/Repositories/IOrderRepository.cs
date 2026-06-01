using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(Order order);
    Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? status = null,
        int? customerId = null,
        string? customerSearch = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null);
    Task<List<Order>> GetByCustomerIdAsync(int customerId);
    Task SaveChangesAsync();
}
