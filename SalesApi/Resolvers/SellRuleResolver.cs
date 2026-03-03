using SalesApi.Models.Enums;
using SalesApi.Repositories;
using SalesApi.Rules;

namespace SalesApi.Resolvers;

public class SellRuleResolver : ISellRuleResolver
{
    private readonly ICustomerRepository _customerRepository;

    public SellRuleResolver(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public ISellRule Resolve(CustomerType customerType) => customerType switch
    {
        CustomerType.Vip => new VipRule(),
        CustomerType.Intern => new InternRule(_customerRepository),
        _ => new CustomerRule()
    };
}
