using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context) => _context = context;

    public Task<Cart?> GetByCustomerIdAsync(int customerId) =>
        _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

    public async Task<Cart> GetOrCreateAsync(int customerId)
    {
        var existing = await GetByCustomerIdAsync(customerId);
        if (existing is not null)
            return existing;

        var cart = new Cart { CustomerId = customerId, UpdatedAt = DateTime.UtcNow };
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();
        return (await GetByCustomerIdAsync(customerId))!;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task ClearItemsAsync(int cartId)
    {
        var items = await _context.CartItems.Where(i => i.CartId == cartId).ToListAsync();
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
