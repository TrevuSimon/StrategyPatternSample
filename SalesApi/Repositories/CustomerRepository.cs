using Microsoft.EntityFrameworkCore;
using SalesApi.Data;
using SalesApi.Models.Entities;

namespace SalesApi.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly SalesDbContext _context;

    public CustomerRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<bool> HasUsedInternDiscountThisMonthAsync(Guid customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer?.LastInternDiscountUsed == null) return false;

        var now = DateTime.UtcNow;
        var lastUsed = customer.LastInternDiscountUsed.Value;
        return lastUsed.Year == now.Year && lastUsed.Month == now.Month;
    }

    public async Task MarkInternDiscountUsedAsync(Guid customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            customer.LastInternDiscountUsed = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
