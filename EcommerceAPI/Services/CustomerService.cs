using EcommerceAPI.DTOs;
using EcommerceAPI.Mapping;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories;

namespace EcommerceAPI.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IOrderRepository _orderRepository;

    public CustomerService(ICustomerRepository repository, IOrderRepository orderRepository)
    {
        _repository = repository;
        _orderRepository = orderRepository;
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

    public async Task<List<OrderDTO>> GetOrdersForCustomerAsync(int customerId)
    {
        if (customerId <= 0)
            throw new ArgumentException("Customer ID must be greater than 0");

        var customer = await _repository.GetByIdAsync(customerId);
        if (customer is null)
            throw new ArgumentException($"Customer with ID {customerId} not found");

        var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
        return orders.Select(OrderMapper.ToDto).ToList();
    }

    public async Task<IReadOnlyList<CustomerPickerDTO>> GetStorefrontPickerAsync(CancellationToken cancellationToken = default)
    {
        var list = await _repository.GetStorefrontPickerAsync(cancellationToken);
        return list;
    }

    private static CustomerDTO Map(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone
    };
}
