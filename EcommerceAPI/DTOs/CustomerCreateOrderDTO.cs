namespace EcommerceAPI.DTOs;

/// <summary>Checkout payload for authenticated customers (customer id comes from JWT).</summary>
public class CustomerCreateOrderDTO
{
    public List<CreateOrderItemDTO> Items { get; set; } = new();
    public string? PromotionCode { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
}
