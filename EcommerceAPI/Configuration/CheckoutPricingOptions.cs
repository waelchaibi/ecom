namespace EcommerceAPI.Configuration;

public sealed class CheckoutPricingOptions
{
    public const string SectionName = "Checkout";

    /// <summary>VAT/sales tax rate applied to subtotal (e.g. 0.20 = 20%).</summary>
    public decimal TaxRate { get; set; } = 0.20m;

    /// <summary>Flat shipping fee when subtotal is below free-shipping threshold.</summary>
    public decimal FlatShippingAmount { get; set; } = 5.99m;

    /// <summary>Subtotal at or above this value gets free shipping.</summary>
    public decimal FreeShippingSubtotalThreshold { get; set; } = 100m;
}
