using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services;

public interface ICustomerService
{
    Task<List<CustomerDTO>> GetAllAsync();
    Task<CustomerDTO?> GetByIdAsync(int id);
}
