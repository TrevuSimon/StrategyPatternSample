namespace SalesApi.Models.DTOs;

public class ProductProcessingData
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal OriginalPrice { get; set; }
    public int Quantity { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal TotalAmount => FinalPrice * Quantity;
    public bool IsValid { get; set; } = true;
    public string? ValidationError { get; set; }
}
