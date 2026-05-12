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

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
