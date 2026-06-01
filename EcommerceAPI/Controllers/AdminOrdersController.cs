using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IAdminOrderService _adminOrderService;
    private readonly IOrderService _orderService;
    private readonly IAuditService _audit;

    public AdminOrdersController(
        IAdminOrderService adminOrderService,
        IOrderService orderService,
        IAuditService audit)
    {
        _adminOrderService = adminOrderService;
        _orderService = orderService;
        _audit = audit;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDTO<AdminOrderListItemDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDTO<AdminOrderListItemDTO>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] int? customerId = null,
        [FromQuery] string? customerSearch = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null)
    {
        var filter = new AdminOrderFilterDTO
        {
            Page = page,
            PageSize = pageSize,
            Status = status,
            CustomerId = customerId,
            CustomerSearch = customerSearch,
            FromUtc = fromUtc,
            ToUtc = toUtc
        };
        return Ok(await _adminOrderService.ListOrdersAsync(filter));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDTO>> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order is null)
            return NotFound();
        return Ok(order);
    }

    [HttpPost("{id:int}/confirm-payment")]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderDTO>> ConfirmPayment(int id)
    {
        var order = await _orderService.ConfirmPaymentAsync(id);
        await _audit.LogAsync(User.Identity?.Name ?? "admin", "ConfirmPayment", "Order", id);
        return Ok(order);
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderDTO>> Cancel(int id)
    {
        var order = await _orderService.CancelOrderAsync(id);
        await _audit.LogAsync(User.Identity?.Name ?? "admin", "Cancel", "Order", id);
        return Ok(order);
    }
}
