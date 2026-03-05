using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using SalesApi.Models.DTOs;
using SalesApi.Models.Entities;
using SalesApi.Rules;

namespace SalesApi.Tests.Rules;

public class CustomerRuleTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ISellProductRule> _productRuleMock;
    private readonly CustomerRule _sut;

    public CustomerRuleTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRuleMock = _fixture.Freeze<Mock<ISellProductRule>>();

        _productRuleMock
            .Setup(x => x.ApplyRuleAsync(It.IsAny<ProductProcessingData>()))
            .ReturnsAsync((ProductProcessingData p) => p);

        _sut = new CustomerRule(_productRuleMock.Object);
    }

    private SaleProcessingData CreateValidSaleData(decimal amount, int itemsCount)
    {
        return new SaleProcessingData
        {
            Customer = _fixture.Create<Customer>(),
            OriginalAmount = amount,
            ItemsCount = itemsCount,
            Products = new List<ProductProcessingData>()
        };
    }

    [Fact]
    public async Task ApplyRuleAsync_ShouldApplyNoDiscount()
    {
        // Arrange
        var data = CreateValidSaleData(100m, 5);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.DiscountPercentage.Should().Be(0m);
        result.FinalAmount.Should().Be(100m);
        result.RuleApplied.Should().Be("CustomerRule");
    }

    [Fact]
    public async Task ApplyRuleAsync_WithInvalidAmount_ShouldReturnInvalid()
    {
        // Arrange
        var data = CreateValidSaleData(0m, 5);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationError.Should().Contain("Amount");
    }

    [Fact]
    public async Task ApplyRuleAsync_WithTooManyItems_ShouldReturnInvalid()
    {
        // Arrange
        var data = CreateValidSaleData(100m, 11); // CustomerRule allows max 10 items

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationError.Should().Contain("Items");
    }

    [Fact]
    public async Task ApplyRuleAsync_WithZeroItems_ShouldReturnInvalid()
    {
        // Arrange
        var data = CreateValidSaleData(100m, 0);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationError.Should().Contain("Items");
    }

    [Fact]
    public void RuleName_ShouldReturnCustomerRule()
    {
        _sut.RuleName.Should().Be("CustomerRule");
    }
}
