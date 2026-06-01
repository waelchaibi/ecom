using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface ICustomerAuthService
{
    Task<CustomerAuthResponseDTO> RegisterAsync(CustomerRegisterDTO dto, CancellationToken cancellationToken = default);
    Task<CustomerAuthResponseDTO> LoginAsync(CustomerLoginDTO dto, CancellationToken cancellationToken = default);
    Task<CustomerProfileDTO?> GetProfileAsync(int customerId, CancellationToken cancellationToken = default);
}
