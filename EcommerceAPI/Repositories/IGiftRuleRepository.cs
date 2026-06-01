using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface IGiftRuleRepository
{
    Task<List<GiftRule>> GetAllWithGiftAsync();
    Task<GiftRule?> GetByIdWithGiftAsync(int id);
    Task<GiftRule> AddAsync(GiftRule rule);
    Task DeleteAsync(GiftRule rule);
    Task<bool> HasOrderGiftsAsync(int giftRuleId);
    Task<bool> HasConflictingActiveRuleAsync(GiftRuleType ruleType, string conditionValue, int giftId, int? excludeRuleId);
    Task SaveChangesAsync();
}
