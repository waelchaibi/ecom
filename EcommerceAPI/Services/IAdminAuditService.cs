using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface IAdminAuditService
{
    Task<PagedResultDTO<AuditLogDTO>> ListAsync(AuditLogFilterDTO filter);
}
