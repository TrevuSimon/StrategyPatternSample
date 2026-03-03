namespace SalesApi.Rules;

public class CustomerRule : BaseSellRule
{
    public override string RuleName => "CustomerRule";
    protected override int DefaultMaxItems => 10;
    protected override decimal DefaultDiscountPercentage => 0m;
}
