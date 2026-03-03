using SalesApi.Models.Enums;

namespace SalesApi.Models.DTOs;

public class SaleRequestDto
{
    public Guid CustomerId { get; set; }
    public CustomerType CustomerType { get; set; }
    public decimal Amount { get; set; }
    public int ItemsCount { get; set; }
}
