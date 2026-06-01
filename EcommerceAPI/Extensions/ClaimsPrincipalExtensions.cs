using System.Security.Claims;
using EcommerceAPI.Models;

namespace EcommerceAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetCustomerId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(AuthClaimTypes.CustomerId);
        if (int.TryParse(value, out var id) && id > 0)
            return id;
        throw new UnauthorizedAccessException("Customer identity is required.");
    }
}
