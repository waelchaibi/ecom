namespace EcommerceAPI.DTOs;

public class AdminOrderFilterDTO
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerSearch { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}
