using SalesApi.Models.DTOs;

namespace SalesApi.Rules;

public class VipRule : BaseSellRule
{
    public VipRule(ISellProductRule productRule) : base(productRule) { }

    public override string RuleName => "VipRule";
    protected override int DefaultMaxItems => 50;
    protected override decimal DefaultDiscountPercentage => 15m;

    // Tiered: 15% base, 20% for $500+, 25% for $1000+, +5% for 10+ items
    protected override Task<decimal> CalculateDiscountAsync(SaleProcessingData data)
    {
        decimal discount = data.OriginalAmount switch
        {
            >= 1000 => 25m,
            >= 500 => 20m,
            _ => DefaultDiscountPercentage
        };

        if (data.ItemsCount >= 10)
            discount += 5m;

        return Task.FromResult(discount);
    }
}
