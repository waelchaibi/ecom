using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDTO>> GetAllAsync();
    Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto);
    Task<CategoryDTO> UpdateAsync(int id, UpdateCategoryDTO dto);
    Task DeleteAsync(int id);
}
