using EcommerceAPI.Data;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync() =>
        await _context.Customers.AsNoTracking().OrderBy(c => c.Id).ToListAsync();

    public async Task<Customer?> GetByIdAsync(int id) =>
        await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<CustomerPickerDTO>> GetStorefrontPickerAsync(CancellationToken cancellationToken = default) =>
        await _context.Customers.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CustomerPickerDTO { Id = c.Id, Name = c.Name })
            .ToListAsync(cancellationToken);
}
