using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories;

public class GiftRuleRepository : IGiftRuleRepository
{
    private readonly AppDbContext _context;

    public GiftRuleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GiftRule>> GetAllWithGiftAsync() =>
        await _context.GiftRules
            .Include(r => r.Gift)
            .OrderByDescending(r => r.Priority)
            .ThenBy(r => r.Id)
            .ToListAsync();

    public async Task<GiftRule?> GetByIdWithGiftAsync(int id) =>
        await _context.GiftRules
            .Include(r => r.Gift)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<GiftRule> AddAsync(GiftRule rule)
    {
        _context.GiftRules.Add(rule);
        await SaveChangesAsync();
        return rule;
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
