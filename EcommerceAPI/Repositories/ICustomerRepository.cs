using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
}
