using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using SalesApi.Models.DTOs;
using SalesApi.Models.Entities;
using SalesApi.Rules;

namespace SalesApi.Tests.Rules;

public class VipRuleTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ISellProductRule> _productRuleMock;
    private readonly VipRule _sut;

    public VipRuleTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRuleMock = _fixture.Freeze<Mock<ISellProductRule>>();
        
        _productRuleMock
            .Setup(x => x.ApplyRuleAsync(It.IsAny<ProductProcessingData>()))
            .ReturnsAsync((ProductProcessingData p) => p);

        _sut = new VipRule(_productRuleMock.Object);
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
    public async Task ApplyRuleAsync_WithAmountUnder500_ShouldApply15PercentDiscount()
    {
        // Arrange
        var data = CreateValidSaleData(400m, 5);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.DiscountPercentage.Should().Be(15m);
        result.FinalAmount.Should().Be(340m); // 400 - 15%
        result.RuleApplied.Should().Be("VipRule");
    }

    [Fact]
    public async Task ApplyRuleAsync_WithAmountBetween500And1000_ShouldApply20PercentDiscount()
    {
        // Arrange
        var data = CreateValidSaleData(600m, 5);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.DiscountPercentage.Should().Be(20m);
        result.FinalAmount.Should().Be(480m); // 600 - 20%
    }

    [Fact]
    public async Task ApplyRuleAsync_WithAmountOver1000_ShouldApply25PercentDiscount()
    {
        // Arrange
        var data = CreateValidSaleData(1000m, 5);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.DiscountPercentage.Should().Be(25m);
        result.FinalAmount.Should().Be(750m); // 1000 - 25%
    }

    [Fact]
    public async Task ApplyRuleAsync_With10OrMoreItems_ShouldAddExtra5PercentDiscount()
    {
        // Arrange
        var data = CreateValidSaleData(400m, 10);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.DiscountPercentage.Should().Be(20m); // 15% base + 5% items bonus
        result.FinalAmount.Should().Be(320m); // 400 - 20%
    }

    [Fact]
    public async Task ApplyRuleAsync_With1000AndOver10Items_ShouldApply30PercentDiscount()
    {
        // Arrange
        var data = CreateValidSaleData(1000m, 15);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.DiscountPercentage.Should().Be(30m); // 25% + 5%
        result.FinalAmount.Should().Be(700m); // 1000 - 30%
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
        var data = CreateValidSaleData(500m, 51); // VipRule allows max 50 items

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationError.Should().Contain("Items");
    }

    [Fact]
    public async Task ApplyRuleAsync_WithProducts_ShouldApplyProductRules()
    {
        // Arrange
        var data = CreateValidSaleData(500m, 2);
        data.Products.Add(new ProductProcessingData
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Test Product",
            OriginalPrice = 100m,
            Quantity = 2
        });

        // Act
        await _sut.ApplyRuleAsync(data);

        // Assert
        _productRuleMock.Verify(
            x => x.ApplyRuleAsync(It.IsAny<ProductProcessingData>()), 
            Times.Once);
    }

    [Fact]
    public void RuleName_ShouldReturnVipRule()
    {
        _sut.RuleName.Should().Be("VipRule");
    }
}
