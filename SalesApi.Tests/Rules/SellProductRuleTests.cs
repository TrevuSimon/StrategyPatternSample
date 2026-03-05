using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using SalesApi.Models.DTOs;
using SalesApi.Rules;

namespace SalesApi.Tests.Rules;

public class SellProductRuleTests
{
    private readonly IFixture _fixture;
    private readonly SellProductRule _sut;

    public SellProductRuleTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _sut = new SellProductRule();
    }

    private ProductProcessingData CreateValidProductData(decimal price, int quantity)
    {
        return new ProductProcessingData
        {
            ProductId = Guid.NewGuid(),
            ProductName = _fixture.Create<string>(),
            OriginalPrice = price,
            Quantity = quantity
        };
    }

    [Fact]
    public async Task ApplyRuleAsync_WithValidProduct_ShouldApplyNoDiscount()
    {
        // Arrange
        var data = CreateValidProductData(100m, 2);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeTrue();
        result.DiscountPercentage.Should().Be(0m);
        result.FinalPrice.Should().Be(100m);
        result.TotalAmount.Should().Be(200m); // 100 * 2
    }

    [Fact]
    public async Task ApplyRuleAsync_WithZeroPrice_ShouldReturnInvalid()
    {
        // Arrange
        var data = CreateValidProductData(0m, 2);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationError.Should().Contain("price");
    }

    [Fact]
    public async Task ApplyRuleAsync_WithNegativePrice_ShouldReturnInvalid()
    {
        // Arrange
        var data = CreateValidProductData(-50m, 2);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationError.Should().Contain("price");
    }

    [Fact]
    public async Task ApplyRuleAsync_WithZeroQuantity_ShouldReturnInvalid()
    {
        // Arrange
        var data = CreateValidProductData(100m, 0);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationError.Should().Contain("quantity");
    }

    [Fact]
    public async Task ApplyRuleAsync_WithNegativeQuantity_ShouldReturnInvalid()
    {
        // Arrange
        var data = CreateValidProductData(100m, -1);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationError.Should().Contain("quantity");
    }

    [Fact]
    public void RuleName_ShouldReturnSellProductRule()
    {
        _sut.RuleName.Should().Be("SellProductRule");
    }
}
