namespace EcommerceAPI.Services;

public interface IAuditService
{
    Task LogAsync(string adminUsername, string action, string entityType, int? entityId, string? details = null);
}
