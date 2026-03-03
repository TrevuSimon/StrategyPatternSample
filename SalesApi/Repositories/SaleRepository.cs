using Microsoft.EntityFrameworkCore;
using SalesApi.Data;
using SalesApi.Models.Entities;

namespace SalesApi.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly SalesDbContext _context;

    public SaleRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateSaleAsync(Sale sale)
    {
        sale.Id = Guid.NewGuid();
        sale.CreatedAt = DateTime.UtcNow;
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
        return sale;
    }

    public async Task<Sale?> GetSaleByIdAsync(Guid id)
    {
        return await _context.Sales.FindAsync(id);
    }

    public async Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId)
    {
        return await _context.Sales
            .Where(s => s.CustomerId == customerId)
            .ToListAsync();
    }
}
