using SalesApi.Models.Entities;

namespace SalesApi.Models.DTOs;

public class SaleProcessingData
{
    public Customer Customer { get; set; } = null!;
    public decimal OriginalAmount { get; set; }
    public int ItemsCount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal FinalAmount { get; set; }
    public int MaxItemsAllowed { get; set; }
    public bool IsValid { get; set; } = true;
    public string? ValidationError { get; set; }
    public string RuleApplied { get; set; } = string.Empty;
    public List<ProductProcessingData> Products { get; set; } = new();
}
