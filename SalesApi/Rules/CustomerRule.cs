namespace SalesApi.Rules;

public class CustomerRule : BaseSellRule
{
    public CustomerRule(ISellProductRule productRule) : base(productRule) { }

    public override string RuleName => "CustomerRule";
    protected override int DefaultMaxItems => 10;
    protected override decimal DefaultDiscountPercentage => 0m;
}
