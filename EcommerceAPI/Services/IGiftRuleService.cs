using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface IGiftRuleService
{
    Task<List<GiftRuleDTO>> GetAllAsync();
    Task<GiftRuleDTO> CreateAsync(CreateGiftRuleDTO dto);
    Task<GiftRuleDTO> UpdateAsync(int id, UpdateGiftRuleDTO dto);
    Task<GiftRuleDTO> SetActiveAsync(int id, bool isActive);
    Task DeleteAsync(int id);
}
