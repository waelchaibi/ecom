using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface IGiftService
{
    Task<List<GiftDTO>> GetAllAsync();
    Task<GiftDTO?> GetByIdAsync(int id);
    Task<GiftDTO> CreateAsync(CreateGiftDTO dto);
}
