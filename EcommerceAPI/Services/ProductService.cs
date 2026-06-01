using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Mapping;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly AppDbContext _context;

    public ProductService(IProductRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<List<ProductDTO>> GetAllProductsAsync(int? categoryId = null)
    {
        var products = await _repository.GetAllAsync(categoryId);
        return products.Select(ProductMapper.ToDto).ToList();
    }

    public async Task<ProductDTO?> GetProductByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than 0");

        var product = await _repository.GetByIdAsync(id);
        return product is null ? null : ProductMapper.ToDto(product);
    }

    public async Task<ProductDTO> CreateProductAsync(CreateProductDTO dto) =>
        ProductMapper.ToDto(await CreateProductEntityAsync(dto));

    public async Task<ProductDTO> UpdateProductAsync(int id, UpdateProductDTO dto)
    {
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than 0");

        ValidateProductFields(dto.Name, dto.Price, dto.StockQuantity);
        await ValidateCategoryAsync(dto.CategoryId);

        var product = await _repository.GetByIdAsync(id);
        if (product is null)
            throw new ArgumentException($"Product with ID {id} not found");

        product.Name = dto.Name.Trim();
        product.Description = dto.Description?.Trim() ?? string.Empty;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.CategoryId = dto.CategoryId;
        product.ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim();
        product.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(product);
        return ProductMapper.ToDto(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than 0");

        var product = await _repository.GetByIdAsync(id);
        if (product is null)
            throw new ArgumentException($"Product with ID {id} not found");

        if (await _repository.HasOrderItemsAsync(id))
            throw new InvalidOperationException("Cannot delete a product that appears on existing orders.");

        await _repository.DeleteAsync(product);
    }

    public async Task<ProductDTO> UpdateStockAsync(int id, int stockQuantity)
    {
        if (id <= 0)
            throw new ArgumentException("Product ID must be greater than 0");

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative");

        var product = await _repository.GetByIdAsync(id);
        if (product is null)
            throw new ArgumentException($"Product with ID {id} not found");

        product.StockQuantity = stockQuantity;
        product.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(product);
        return ProductMapper.ToDto(product);
    }

    private async Task<Product> CreateProductEntityAsync(CreateProductDTO dto)
    {
        ValidateProductFields(dto.Name, dto.Price, dto.StockQuantity);
        await ValidateCategoryAsync(dto.CategoryId);

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CategoryId = dto.CategoryId,
            ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim()
        };

        return await _repository.CreateAsync(product);
    }

    private static void ValidateProductFields(string name, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required");

        if (price <= 0)
            throw new ArgumentException("Product price must be greater than 0");

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative");
    }

    private async Task ValidateCategoryAsync(int? categoryId)
    {
        if (!categoryId.HasValue)
            return;

        var exists = await _context.Categories.AnyAsync(c => c.Id == categoryId.Value);
        if (!exists)
            throw new ArgumentException($"Category with ID {categoryId} not found");
    }
}
