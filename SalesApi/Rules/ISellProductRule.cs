using SalesApi.Models.DTOs;

namespace SalesApi.Rules;

public interface ISellProductRule
{
    Task<ProductProcessingData> ApplyRuleAsync(ProductProcessingData data);
    string RuleName { get; }
}
