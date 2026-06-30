namespace EcommerceAPI.DTOs;

public class CheckoutPricingDTO
{
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxRate { get; set; }
    public bool FreeShippingApplied { get; set; }
}
