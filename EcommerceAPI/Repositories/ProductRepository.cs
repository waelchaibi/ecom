using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(int? categoryId = null, bool activeOnly = true)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (activeOnly)
            query = query.Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id) =>
        await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await SaveChangesAsync();
    }

    public async Task<bool> HasOrderItemsAsync(int productId) =>
        await _context.OrderItems.AnyAsync(oi => oi.ProductId == productId);

    public async Task RemoveFromAllCartsAsync(int productId)
    {
        var lines = await _context.CartItems.Where(i => i.ProductId == productId).ToListAsync();
        if (lines.Count == 0)
            return;
        _context.CartItems.RemoveRange(lines);
        await SaveChangesAsync();
    }

    public async Task<bool> TryDecrementStockAsync(
        int productId,
        int quantity,
        DateTime updatedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"UPDATE ""Products"" SET ""StockQuantity"" = ""StockQuantity"" - {quantity}, ""UpdatedAt"" = {updatedAtUtc}
               WHERE ""Id"" = {productId} AND ""IsActive"" = TRUE AND ""StockQuantity"" >= {quantity}",
            cancellationToken);
        return rows == 1;
    }

    public async Task IncrementStockAsync(
        int productId,
        int quantity,
        DateTime updatedAtUtc,
        CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"UPDATE ""Products"" SET ""StockQuantity"" = ""StockQuantity"" + {quantity}, ""UpdatedAt"" = {updatedAtUtc} WHERE ""Id"" = {productId}",
            cancellationToken);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
