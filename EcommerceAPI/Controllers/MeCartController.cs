using EcommerceAPI.DTOs;
using EcommerceAPI.Extensions;
using EcommerceAPI.Models;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/me/cart")]
[Authorize(Roles = AuthRoles.Customer)]
public class MeCartController : ControllerBase
{
    private readonly ICartService _cartService;

    public MeCartController(ICartService cartService) => _cartService = cartService;

    [HttpGet]
    public async Task<ActionResult<CartDTO>> Get() =>
        Ok(await _cartService.GetCartAsync(User.GetCustomerId()));

    [HttpPost("items")]
    public async Task<ActionResult<CartDTO>> UpsertItem([FromBody] CartItemUpsertDTO dto) =>
        Ok(await _cartService.UpsertItemAsync(User.GetCustomerId(), dto));

    [HttpDelete("items/{productId:int}")]
    public async Task<ActionResult<CartDTO>> RemoveItem(int productId) =>
        Ok(await _cartService.RemoveItemAsync(User.GetCustomerId(), productId));

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        await _cartService.ClearCartAsync(User.GetCustomerId());
        return NoContent();
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<OrderDTO>> Checkout([FromBody] CartCheckoutDTO dto) =>
        Ok(await _cartService.CheckoutAsync(User.GetCustomerId(), dto));
}
