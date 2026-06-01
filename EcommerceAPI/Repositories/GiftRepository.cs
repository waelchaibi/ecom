using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories;

public class GiftRepository : IGiftRepository
{
    private readonly AppDbContext _context;

    public GiftRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Gift>> GetAllAsync() =>
        await _context.Gifts.OrderBy(g => g.Id).ToListAsync();

    public async Task<Gift?> GetByIdAsync(int id) =>
        await _context.Gifts.FindAsync(id);

    public async Task<Gift> AddAsync(Gift gift)
    {
        _context.Gifts.Add(gift);
        await SaveChangesAsync();
        return gift;
    }

    public async Task<bool> TryDecrementStockAsync(int giftId, int quantity, CancellationToken cancellationToken = default)
    {
        var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"UPDATE ""Gifts"" SET ""StockQuantity"" = ""StockQuantity"" - {quantity} WHERE ""Id"" = {giftId} AND ""StockQuantity"" >= {quantity}",
            cancellationToken);
        return rows == 1;
    }

    public async Task IncrementStockAsync(int giftId, int quantity, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"UPDATE ""Gifts"" SET ""StockQuantity"" = ""StockQuantity"" + {quantity} WHERE ""Id"" = {giftId}",
            cancellationToken);
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
