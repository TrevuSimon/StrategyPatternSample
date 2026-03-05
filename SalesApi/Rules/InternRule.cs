using SalesApi.Models.DTOs;
using SalesApi.Repositories;

namespace SalesApi.Rules;

public class InternRule : BaseSellRule
{
    private readonly ICustomerRepository _customerRepository;

    public InternRule(ICustomerRepository customerRepository, ISellProductRule productRule) 
        : base(productRule)
    {
        _customerRepository = customerRepository;
    }

    public override string RuleName => "InternRule";
    protected override int DefaultMaxItems => 15;
    protected override decimal DefaultDiscountPercentage => 30m;

    // 30% if not used this month, otherwise 0%
    protected override async Task<decimal> CalculateDiscountAsync(SaleProcessingData data)
    {
        var hasUsedDiscount = await _customerRepository
            .HasUsedInternDiscountThisMonthAsync(data.Customer.Id);

        return hasUsedDiscount ? 0m : DefaultDiscountPercentage;
    }

    protected override async Task OnAfterApplyAsync(SaleProcessingData data)
    {
        if (data.DiscountPercentage > 0)
            await _customerRepository.MarkInternDiscountUsedAsync(data.Customer.Id);
    }
}
