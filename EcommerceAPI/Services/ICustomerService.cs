using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface ICustomerService
{
    Task<List<CustomerDTO>> GetAllAsync();
    Task<CustomerDTO?> GetByIdAsync(int id);
    Task<List<OrderDTO>> GetOrdersForCustomerAsync(int customerId);
    Task<IReadOnlyList<CustomerPickerDTO>> GetStorefrontPickerAsync(CancellationToken cancellationToken = default);
}
