using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<CategoryDTO>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(Map).ToList();
    }

    public async Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Category name is required");

        var created = await _repository.CreateAsync(new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim()
        });
        return Map(created);
    }

    public async Task<CategoryDTO> UpdateAsync(int id, UpdateCategoryDTO dto)
    {
        if (id <= 0)
            throw new ArgumentException("Category ID must be greater than 0");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Category name is required");

        var category = await _repository.GetByIdAsync(id);
        if (category is null)
            throw new KeyNotFoundException($"Category with ID {id} not found");

        category.Name = dto.Name.Trim();
        category.Description = dto.Description?.Trim();
        await _repository.UpdateAsync(category);
        return Map(category);
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Category ID must be greater than 0");

        var category = await _repository.GetByIdAsync(id);
        if (category is null)
            throw new KeyNotFoundException($"Category with ID {id} not found");

        if (await _repository.HasProductsAsync(id))
            throw new InvalidOperationException("Cannot delete a category that still has products.");

        await _repository.DeleteAsync(category);
    }

    private static CategoryDTO Map(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description
    };
}
