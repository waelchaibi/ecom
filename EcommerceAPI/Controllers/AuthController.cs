using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EcommerceAPI.Configuration;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AdminCredentialsOptions _admin;
    private readonly IJwtTokenService _jwt;

    public AuthController(IOptions<AdminCredentialsOptions> adminOptions, IJwtTokenService jwt)
    {
        _admin = adminOptions.Value;
        _jwt = jwt;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Login([FromBody] LoginRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(_admin.Password))
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "Admin password is not configured. Set Admin:Password in configuration." });

        if (request.Username != _admin.Username || request.Password != _admin.Password)
            throw new UnauthorizedAccessException("Invalid username or password.");

        var token = _jwt.CreateAdminToken(request.Username);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return Ok(new LoginResponseDTO
        {
            Token = token,
            ExpiresAtUtc = jwt.ValidTo
        });
    }
}
