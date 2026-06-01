using System.IdentityModel.Tokens.Jwt;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public sealed class CustomerAuthService : ICustomerAuthService
{
    private readonly ICustomerRepository _customers;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IJwtTokenService _jwt;

    public CustomerAuthService(
        ICustomerRepository customers,
        IPasswordHasherService passwordHasher,
        IJwtTokenService jwt)
    {
        _customers = customers;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
    }

    public async Task<CustomerAuthResponseDTO> RegisterAsync(
        CustomerRegisterDTO dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");

        var email = NormalizeEmail(dto.Email);
        var existing = await _customers.GetByEmailAsync(email, cancellationToken);

        Customer customer;
        if (existing is not null)
        {
            if (!string.IsNullOrEmpty(existing.PasswordHash))
                throw new InvalidOperationException("An account with this email already exists. Sign in instead.");

            existing.Name = dto.Name.Trim();
            existing.Phone = dto.Phone?.Trim() ?? string.Empty;
            existing.PasswordHash = _passwordHasher.Hash(dto.Password);
            await _customers.UpdateAsync(existing, cancellationToken);
            customer = existing;
        }
        else
        {
            customer = new Customer
            {
                Name = dto.Name.Trim(),
                Email = email,
                Phone = dto.Phone?.Trim() ?? string.Empty,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                CreatedAt = DateTime.UtcNow
            };
            customer = await _customers.AddAsync(customer, cancellationToken);
        }

        return BuildAuthResponse(customer);
    }

    public async Task<CustomerAuthResponseDTO> LoginAsync(
        CustomerLoginDTO dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Email and password are required.");

        var email = NormalizeEmail(dto.Email);
        var customer = await _customers.GetByEmailAsync(email, cancellationToken);
        if (customer is null || !_passwordHasher.Verify(dto.Password, customer.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return BuildAuthResponse(customer);
    }

    public async Task<CustomerProfileDTO?> GetProfileAsync(int customerId, CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
            throw new ArgumentException("Customer ID must be greater than 0.");

        var customer = await _customers.GetByIdAsync(customerId);
        return customer is null ? null : MapProfile(customer);
    }

    private CustomerAuthResponseDTO BuildAuthResponse(Customer customer)
    {
        var token = _jwt.CreateCustomerToken(customer.Id, customer.Email, customer.Name);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return new CustomerAuthResponseDTO
        {
            Token = token,
            ExpiresAtUtc = jwt.ValidTo,
            Customer = MapProfile(customer)
        };
    }

    private static CustomerProfileDTO MapProfile(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone
    };

    internal static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
