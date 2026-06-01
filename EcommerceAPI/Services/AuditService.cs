using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public sealed class AuditService : IAuditService
{
    private readonly IAuditLogRepository _repository;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IAuditLogRepository repository, ILogger<AuditService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task LogAsync(string adminUsername, string action, string entityType, int? entityId, string? details = null)
    {
        var entry = new AuditLog
        {
            AdminUsername = adminUsername,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entry);
        _logger.LogInformation(
            "Audit {Action} on {EntityType} {EntityId} by {Admin}",
            action,
            entityType,
            entityId,
            adminUsername);
    }
}
