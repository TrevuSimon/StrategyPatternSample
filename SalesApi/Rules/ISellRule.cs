using SalesApi.Models.DTOs;

namespace SalesApi.Rules;

public interface ISellRule
{
    Task<SaleProcessingData> ApplyRuleAsync(SaleProcessingData data);
    string RuleName { get; }
}
