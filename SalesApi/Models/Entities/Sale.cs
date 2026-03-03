namespace SalesApi.Models.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal DiscountApplied { get; set; }
    public decimal FinalAmount { get; set; }
    public int ItemsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string RuleApplied { get; set; } = string.Empty;
}
