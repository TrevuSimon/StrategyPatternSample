using SalesApi.Models.Enums;
using SalesApi.Repositories;
using SalesApi.Rules;

namespace SalesApi.Resolvers;

public class SellRuleResolver : ISellRuleResolver
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ISellProductRule _productRule;

    public SellRuleResolver(
        ICustomerRepository customerRepository,
        ISellProductRule productRule)
    {
        _customerRepository = customerRepository;
        _productRule = productRule;
    }

    public ISellRule Resolve(CustomerType customerType) => customerType switch
    {
        CustomerType.Vip => new VipRule(_productRule),
        CustomerType.Intern => new InternRule(_customerRepository, _productRule),
        _ => new CustomerRule(_productRule)
    };
}
