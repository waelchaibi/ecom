using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) => _context = context;

    public Task<List<Category>> GetAllAsync() =>
        _context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

    public Task<Category?> GetByIdAsync(int id) => _context.Categories.FindAsync(id).AsTask();

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public Task<bool> HasProductsAsync(int categoryId) =>
        _context.Products.AnyAsync(p => p.CategoryId == categoryId);
}
