using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using SalesApi.Models.Enums;
using SalesApi.Repositories;
using SalesApi.Resolvers;
using SalesApi.Rules;

namespace SalesApi.Tests.Resolvers;

public class SellRuleResolverTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<ISellProductRule> _productRuleMock;
    private readonly SellRuleResolver _sut;

    public SellRuleResolverTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _customerRepositoryMock = _fixture.Freeze<Mock<ICustomerRepository>>();
        _productRuleMock = _fixture.Freeze<Mock<ISellProductRule>>();

        _sut = new SellRuleResolver(
            _customerRepositoryMock.Object,
            _productRuleMock.Object);
    }

    [Fact]
    public void Resolve_WithVipCustomerType_ShouldReturnVipRule()
    {
        // Act
        var result = _sut.Resolve(CustomerType.Vip);

        // Assert
        result.Should().BeOfType<VipRule>();
        result.RuleName.Should().Be("VipRule");
    }

    [Fact]
    public void Resolve_WithInternCustomerType_ShouldReturnInternRule()
    {
        // Act
        var result = _sut.Resolve(CustomerType.Intern);

        // Assert
        result.Should().BeOfType<InternRule>();
        result.RuleName.Should().Be("InternRule");
    }

    [Fact]
    public void Resolve_WithCustomerType_ShouldReturnCustomerRule()
    {
        // Act
        var result = _sut.Resolve(CustomerType.Customer);

        // Assert
        result.Should().BeOfType<CustomerRule>();
        result.RuleName.Should().Be("CustomerRule");
    }

    [Theory]
    [InlineData(CustomerType.Vip, typeof(VipRule))]
    [InlineData(CustomerType.Intern, typeof(InternRule))]
    [InlineData(CustomerType.Customer, typeof(CustomerRule))]
    public void Resolve_ShouldReturnCorrectRuleType(CustomerType customerType, Type expectedType)
    {
        // Act
        var result = _sut.Resolve(customerType);

        // Assert
        result.Should().BeOfType(expectedType);
    }
}
