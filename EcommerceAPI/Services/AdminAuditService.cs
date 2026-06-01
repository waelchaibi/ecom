using EcommerceAPI.DTOs;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public sealed class AdminAuditService : IAdminAuditService
{
    private readonly IAuditLogRepository _repository;

    public AdminAuditService(IAuditLogRepository repository) => _repository = repository;

    public async Task<PagedResultDTO<AuditLogDTO>> ListAsync(AuditLogFilterDTO filter)
    {
        var (items, total) = await _repository.GetPagedAsync(
            filter.Page,
            filter.PageSize,
            filter.Action,
            filter.EntityType,
            filter.FromUtc,
            filter.ToUtc);

        return new PagedResultDTO<AuditLogDTO>
        {
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = total,
            Items = items.Select(a => new AuditLogDTO
            {
                Id = a.Id,
                AdminUsername = a.AdminUsername,
                Action = a.Action,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Details = a.Details,
                CreatedAt = a.CreatedAt
            }).ToList()
        };
    }
}
