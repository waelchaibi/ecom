namespace EcommerceAPI.DTOs;

public class CustomerRegisterDTO
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string IdentityCard { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CustomerLoginDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CustomerProfileDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string IdentityCard { get; set; } = string.Empty;
}

public class UpdateCustomerProfileDTO
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string IdentityCard { get; set; } = string.Empty;
}

public class CustomerAuthResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public CustomerProfileDTO Customer { get; set; } = new();
}
