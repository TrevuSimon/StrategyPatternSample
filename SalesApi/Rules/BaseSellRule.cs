using SalesApi.Models.DTOs;

namespace SalesApi.Rules;

public abstract class BaseSellRule : ISellRule
{
    protected readonly ISellProductRule _productRule;

    protected BaseSellRule(ISellProductRule productRule)
    {
        _productRule = productRule;
    }

    protected virtual int DefaultMaxItems => 10;
    protected virtual decimal DefaultDiscountPercentage => 0m;

    public abstract string RuleName { get; }

    public virtual async Task<SaleProcessingData> ApplyRuleAsync(SaleProcessingData data)
    {
        // Validate sale
        if (!Validate(data)) return data;

        // Apply product rules first
        await ApplyProductRulesAsync(data);
        if (!data.IsValid) return data;

        // Calculate sale discount
        data.DiscountPercentage = await CalculateDiscountAsync(data);
        data.MaxItemsAllowed = DefaultMaxItems;
        data.FinalAmount = data.OriginalAmount - (data.OriginalAmount * data.DiscountPercentage / 100);
        data.RuleApplied = RuleName;

        // Custom hook
        await OnAfterApplyAsync(data);

        return data;
    }

    protected virtual async Task ApplyProductRulesAsync(SaleProcessingData data)
    {
        foreach (var product in data.Products)
        {
            await _productRule.ApplyRuleAsync(product);

            if (!product.IsValid)
            {
                data.IsValid = false;
                data.ValidationError = $"Product '{product.ProductName}': {product.ValidationError}";
                return;
            }
        }
    }

    protected virtual bool Validate(SaleProcessingData data)
    {
        if (data.OriginalAmount <= 0)
        {
            data.IsValid = false;
            data.ValidationError = "Amount must be greater than zero.";
            return false;
        }

        if (data.ItemsCount <= 0 || data.ItemsCount > DefaultMaxItems)
        {
            data.IsValid = false;
            data.ValidationError = $"Items must be between 1 and {DefaultMaxItems}.";
            return false;
        }

        return true;
    }

    protected virtual Task<decimal> CalculateDiscountAsync(SaleProcessingData data) 
        => Task.FromResult(DefaultDiscountPercentage);

    protected virtual Task OnAfterApplyAsync(SaleProcessingData data) 
        => Task.CompletedTask;
}
