using SalesApi.Models.Enums;

namespace SalesApi.Models.DTOs;

public class SaleRequestDto
{
    public Guid CustomerId { get; set; }
    public CustomerType CustomerType { get; set; }
    public decimal Amount { get; set; }
    public int ItemsCount { get; set; }
    public List<ProductRequestDto> Products { get; set; } = new();
}

public class ProductRequestDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
