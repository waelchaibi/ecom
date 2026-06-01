namespace EcommerceAPI.DTOs;

public class AuditLogDTO
{
    public int Id { get; set; }
    public string AdminUsername { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuditLogFilterDTO
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Action { get; set; }
    public string? EntityType { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}
