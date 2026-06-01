using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/admin/customers")]
[Authorize(Roles = "Admin")]
public class AdminCustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public AdminCustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustomerDTO>>> GetAll()
    {
        return Ok(await _customerService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CustomerDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDTO>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer is null)
            return NotFound();
        return Ok(customer);
    }

    [HttpGet("{id:int}/orders")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDTO>>> GetOrders(int id)
    {
        return Ok(await _customerService.GetOrdersForCustomerAsync(id));
    }
}
