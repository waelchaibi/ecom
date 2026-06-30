using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface ICheckoutPricingService
{
    CheckoutPricingDTO Calculate(decimal subtotal);
}
