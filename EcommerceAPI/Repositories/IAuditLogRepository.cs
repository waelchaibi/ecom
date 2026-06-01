using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog entry);

    Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? action = null,
        string? entityType = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null);
}
