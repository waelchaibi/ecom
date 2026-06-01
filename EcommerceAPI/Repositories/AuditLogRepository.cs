using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(AuditLog entry)
    {
        _context.AuditLogs.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? action = null,
        string? entityType = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 100) pageSize = 100;

        var query = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action == action.Trim());

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(a => a.EntityType == entityType.Trim());

        if (fromUtc.HasValue)
            query = query.Where(a => a.CreatedAt >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(a => a.CreatedAt <= toUtc.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
