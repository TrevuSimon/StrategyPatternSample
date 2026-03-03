using SalesApi.Models.Entities;

namespace SalesApi.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer> UpdateAsync(Customer customer);
    Task<bool> HasUsedInternDiscountThisMonthAsync(Guid customerId);
    Task MarkInternDiscountUsedAsync(Guid customerId);
}
