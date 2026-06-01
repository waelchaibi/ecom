using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/auth/customer")]
public class CustomerAuthController : ControllerBase
{
    private readonly ICustomerAuthService _customerAuth;

    public CustomerAuthController(ICustomerAuthService customerAuth)
    {
        _customerAuth = customerAuth;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomerAuthResponseDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<CustomerAuthResponseDTO>> Register(
        [FromBody] CustomerRegisterDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _customerAuth.RegisterAsync(dto, cancellationToken));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomerAuthResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CustomerAuthResponseDTO>> Login(
        [FromBody] CustomerLoginDTO dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _customerAuth.LoginAsync(dto, cancellationToken));
    }
}
