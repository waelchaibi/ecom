namespace EcommerceAPI.Models;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";

    /// <summary>Optional promotion code supplied at checkout; used when confirming payment for gift rules.</summary>
    public string? PromotionCode { get; set; }

    // Navigation properties
    public Customer? Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<OrderGift> OrderGifts { get; set; } = new List<OrderGift>();
}