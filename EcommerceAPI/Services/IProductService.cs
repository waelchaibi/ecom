using EcommerceAPI.DTOs;
using EcommerceAPI.Models;

namespace EcommerceAPI.Services;

public interface IProductService
{
    Task<List<ProductDTO>> GetAllProductsAsync(int? categoryId = null);
    Task<ProductDTO?> GetProductByIdAsync(int id);
    Task<ProductDTO> CreateProductAsync(CreateProductDTO dto);
    Task<ProductDTO> UpdateProductAsync(int id, UpdateProductDTO dto);
    Task DeleteProductAsync(int id);
    Task<ProductDTO> UpdateStockAsync(int id, int stockQuantity);
}