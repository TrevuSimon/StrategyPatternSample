using SalesApi.Models.Enums;
using SalesApi.Rules;

namespace SalesApi.Resolvers;

public interface ISellRuleResolver
{
    ISellRule Resolve(CustomerType customerType);
}
