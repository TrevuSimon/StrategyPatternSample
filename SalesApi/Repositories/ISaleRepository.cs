using SalesApi.Models.Entities;

namespace SalesApi.Repositories;

public interface ISaleRepository
{
    Task<Sale> CreateSaleAsync(Sale sale);
    Task<Sale?> GetSaleByIdAsync(Guid id);
    Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId);
}
