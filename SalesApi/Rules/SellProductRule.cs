using SalesApi.Models.DTOs;

namespace SalesApi.Rules;

public class SellProductRule : ISellProductRule
{
    public string RuleName => "SellProductRule";

    public virtual Task<ProductProcessingData> ApplyRuleAsync(ProductProcessingData data)
    {
        if (!Validate(data))
            return Task.FromResult(data);

        data.DiscountPercentage = CalculateDiscount(data);
        data.FinalPrice = data.OriginalPrice - (data.OriginalPrice * data.DiscountPercentage / 100);

        return Task.FromResult(data);
    }

    protected virtual bool Validate(ProductProcessingData data)
    {
        if (data.OriginalPrice <= 0)
        {
            data.IsValid = false;
            data.ValidationError = "Product price must be greater than zero.";
            return false;
        }

        if (data.Quantity <= 0)
        {
            data.IsValid = false;
            data.ValidationError = "Product quantity must be greater than zero.";
            return false;
        }

        return true;
    }

    protected virtual decimal CalculateDiscount(ProductProcessingData data)
    {
        // Base rule: no discount
        return 0m;
    }
}
