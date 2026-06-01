using EcommerceAPI.DTOs;
using EcommerceAPI.Extensions;
using EcommerceAPI.Models;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AuthRoles.Customer)]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDTO>> Create([FromBody] CustomerCreateOrderDTO dto)
    {
        var customerId = User.GetCustomerId();
        var order = await _orderService.CreateOrderForCustomerAsync(customerId, dto);
        return Created($"/api/me/orders/{order.Id}", order);
    }
}
