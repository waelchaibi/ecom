using EcommerceAPI.Configuration;
using EcommerceAPI.DTOs;
using Microsoft.Extensions.Options;

namespace EcommerceAPI.Services;

public sealed class CheckoutPricingService : ICheckoutPricingService
{
    private readonly CheckoutPricingOptions _options;

    public CheckoutPricingService(IOptions<CheckoutPricingOptions> options)
    {
        _options = options.Value;
    }

    public CheckoutPricingDTO Calculate(decimal subtotal)
    {
        if (subtotal < 0)
            throw new ArgumentException("Subtotal cannot be negative.");

        var taxRate = _options.TaxRate;
        if (taxRate < 0)
            throw new InvalidOperationException("Checkout:TaxRate cannot be negative.");

        var tax = RoundMoney(subtotal * taxRate);
        var freeShipping = subtotal >= _options.FreeShippingSubtotalThreshold;
        var shipping = freeShipping ? 0m : RoundMoney(_options.FlatShippingAmount);
        if (shipping < 0)
            throw new InvalidOperationException("Checkout:FlatShippingAmount cannot be negative.");

        return new CheckoutPricingDTO
        {
            Subtotal = RoundMoney(subtotal),
            TaxAmount = tax,
            ShippingAmount = shipping,
            TotalAmount = RoundMoney(subtotal + tax + shipping),
            TaxRate = taxRate,
            FreeShippingApplied = freeShipping
        };
    }

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
