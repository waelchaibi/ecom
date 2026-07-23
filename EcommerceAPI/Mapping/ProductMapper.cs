using EcommerceAPI.DTOs;
using EcommerceAPI.Models;

namespace EcommerceAPI.Mapping;

public static class ProductMapper
{
    public static ProductDTO ToDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        StockQuantity = product.StockQuantity,
        CategoryId = product.CategoryId,
        CategoryName = product.Category?.Name,
        ImageUrl = product.ImageUrl,
        IsActive = product.IsActive
    };
}
