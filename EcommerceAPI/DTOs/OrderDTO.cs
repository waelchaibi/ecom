namespace EcommerceAPI.DTOs;

public class OrderDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemDTO> OrderItems { get; set; } = new();
    public List<OrderGiftDTO> AssignedGifts { get; set; } = new();
}

public class CreateOrderDTO
{
    public int CustomerId { get; set; }
    public List<CreateOrderItemDTO> Items { get; set; } = new();
    /// <summary>Optional promotion code for Promotion-type gift rules.</summary>
    public string? PromotionCode { get; set; }
}
