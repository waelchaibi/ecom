using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutPricingService _pricing;

    public CheckoutController(ICheckoutPricingService pricing)
    {
        _pricing = pricing;
    }

    /// <summary>Server-side tax/shipping estimate for a subtotal (same rules as order creation).</summary>
    [HttpGet("pricing")]
    [ProducesResponseType(typeof(CheckoutPricingDTO), StatusCodes.Status200OK)]
    public ActionResult<CheckoutPricingDTO> GetPricing([FromQuery] decimal subtotal)
    {
        if (subtotal < 0)
            return BadRequest(new { error = "Subtotal cannot be negative." });

        return Ok(_pricing.Calculate(subtotal));
    }
}
