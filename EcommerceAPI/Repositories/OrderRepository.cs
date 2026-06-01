using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.OrderGifts)
                .ThenInclude(og => og.Gift)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? status = null,
        int? customerId = null,
        string? customerSearch = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Orders.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status.Trim());

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        if (!string.IsNullOrWhiteSpace(customerSearch))
        {
            var term = customerSearch.Trim().ToLower();
            query = query.Where(o => _context.Customers.Any(c =>
                c.Id == o.CustomerId &&
                (c.Name.ToLower().Contains(term) || c.Email.ToLower().Contains(term))));
        }

        if (fromUtc.HasValue)
            query = query.Where(o => o.CreatedAt >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(o => o.CreatedAt <= toUtc.Value);

        var total = await query.CountAsync();

        var items = await query
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<Order>> GetByCustomerIdAsync(int customerId) =>
        await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderItems)
            .Include(o => o.OrderGifts)
                .ThenInclude(og => og.Gift)
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await SaveChangesAsync();
        return order;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
