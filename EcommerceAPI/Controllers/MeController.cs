using EcommerceAPI.DTOs;
using EcommerceAPI.Extensions;
using EcommerceAPI.Models;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/me")]
[Authorize(Roles = AuthRoles.Customer)]
public class MeController : ControllerBase
{
    private readonly ICustomerAuthService _customerAuth;
    private readonly IOrderService _orderService;

    public MeController(ICustomerAuthService customerAuth, IOrderService orderService)
    {
        _customerAuth = customerAuth;
        _orderService = orderService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomerProfileDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerProfileDTO>> GetProfile(CancellationToken cancellationToken)
    {
        var customerId = User.GetCustomerId();
        var profile = await _customerAuth.GetProfileAsync(customerId, cancellationToken);
        if (profile is null)
            return NotFound();
        return Ok(profile);
    }

    [HttpPut]
    [ProducesResponseType(typeof(CustomerProfileDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerProfileDTO>> UpdateProfile(
        [FromBody] UpdateCustomerProfileDTO dto,
        CancellationToken cancellationToken)
    {
        var customerId = User.GetCustomerId();
        return Ok(await _customerAuth.UpdateProfileAsync(customerId, dto, cancellationToken));
    }

    [HttpGet("orders")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetMyOrders()
    {
        var customerId = User.GetCustomerId();
        return Ok(await _orderService.GetOrdersForCustomerAsync(customerId));
    }

    [HttpGet("orders/{id:int}")]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDTO>> GetMyOrder(int id)
    {
        var customerId = User.GetCustomerId();
        var order = await _orderService.GetOrderForCustomerAsync(customerId, id);
        if (order is null)
            return NotFound();
        return Ok(order);
    }

    /// <summary>Simulated payment gateway — validates card format then confirms order.</summary>
    [HttpPost("orders/{id:int}/pay")]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDTO>> PayOrder(int id, [FromBody] SimulatePaymentDTO payment) =>
        Ok(await _orderService.PayOrderAsCustomerAsync(User.GetCustomerId(), id, payment));

    [HttpPost("orders/{id:int}/cancel")]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDTO>> CancelOrder(int id) =>
        Ok(await _orderService.CancelOrderAsCustomerAsync(User.GetCustomerId(), id));
}
