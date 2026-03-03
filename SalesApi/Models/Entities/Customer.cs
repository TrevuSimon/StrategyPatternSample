using SalesApi.Models.Enums;

namespace SalesApi.Models.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CustomerType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastInternDiscountUsed { get; set; }
}
