namespace SalesApi.Models.DTOs;

public class SaleResponseDto
{
    public Guid SaleId { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal DiscountApplied { get; set; }
    public decimal FinalAmount { get; set; }
    public int ItemsCount { get; set; }
    public string RuleApplied { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public static SaleResponseDto Failure(string error) => new()
    {
        Success = false,
        ErrorMessage = error
    };
}
