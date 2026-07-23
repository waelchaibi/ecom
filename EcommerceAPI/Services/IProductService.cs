using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface IProductService
{
    Task<List<ProductDTO>> GetAllProductsAsync(int? categoryId = null, bool activeOnly = true);
    Task<ProductDTO?> GetProductByIdAsync(int id, bool activeOnly = true);
    Task<ProductDTO> CreateProductAsync(CreateProductDTO dto);
    Task<ProductDTO> UpdateProductAsync(int id, UpdateProductDTO dto);
    Task SoftDeleteProductAsync(int id);
    Task<ProductDTO> RestoreProductAsync(int id);
    Task<ProductDTO> UpdateStockAsync(int id, int stockQuantity);
}
