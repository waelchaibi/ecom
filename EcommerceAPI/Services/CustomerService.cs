using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CustomerDTO>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(Map).ToList();
    }

    public async Task<CustomerDTO?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Customer ID must be greater than 0");

        var c = await _repository.GetByIdAsync(id);
        return c is null ? null : Map(c);
    }

    private static CustomerDTO Map(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone
    };
}
