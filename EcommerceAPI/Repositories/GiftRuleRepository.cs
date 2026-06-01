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

    public async Task DeleteAsync(GiftRule rule)
    {
        _context.GiftRules.Remove(rule);
        await SaveChangesAsync();
    }

    public async Task<bool> HasOrderGiftsAsync(int giftRuleId) =>
        await _context.OrderGifts.AnyAsync(og => og.GiftRuleId == giftRuleId);

    public async Task<bool> HasConflictingActiveRuleAsync(GiftRuleType ruleType, string conditionValue, int giftId, int? excludeRuleId)
    {
        var normalized = NormalizeCondition(ruleType, conditionValue);
        var query = _context.GiftRules.Where(r => r.IsActive && r.RuleType == ruleType && r.GiftId == giftId);
        if (excludeRuleId.HasValue)
            query = query.Where(r => r.Id != excludeRuleId.Value);

        var candidates = await query.ToListAsync();
        foreach (var r in candidates)
        {
            if (NormalizeCondition(r.RuleType, r.ConditionValue) == normalized)
                return true;
        }

        return false;
    }

    private static string NormalizeCondition(GiftRuleType type, string value)
    {
        var v = value.Trim();
        return type switch
        {
            GiftRuleType.Amount => decimal.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d)
                ? d.ToString(System.Globalization.CultureInfo.InvariantCulture)
                : v,
            GiftRuleType.Promotion => v.ToUpperInvariant(),
            _ => v
        };
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
