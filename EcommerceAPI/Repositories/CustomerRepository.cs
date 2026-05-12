using EcommerceAPI.Data;
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
}
