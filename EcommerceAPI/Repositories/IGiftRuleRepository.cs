using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface IGiftRuleRepository
{
    Task<List<GiftRule>> GetAllWithGiftAsync();
    Task<GiftRule?> GetByIdWithGiftAsync(int id);
    Task<GiftRule> AddAsync(GiftRule rule);
    Task SaveChangesAsync();
}
