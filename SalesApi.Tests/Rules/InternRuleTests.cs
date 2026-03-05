using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using SalesApi.Models.DTOs;
using SalesApi.Models.Entities;
using SalesApi.Repositories;
using SalesApi.Rules;

namespace SalesApi.Tests.Rules;

public class InternRuleTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ISellProductRule> _productRuleMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly InternRule _sut;

    public InternRuleTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _productRuleMock = _fixture.Freeze<Mock<ISellProductRule>>();
        _customerRepositoryMock = _fixture.Freeze<Mock<ICustomerRepository>>();

        _productRuleMock
            .Setup(x => x.ApplyRuleAsync(It.IsAny<ProductProcessingData>()))
            .ReturnsAsync((ProductProcessingData p) => p);

        _sut = new InternRule(_customerRepositoryMock.Object, _productRuleMock.Object);
    }

    private SaleProcessingData CreateValidSaleData(decimal amount, int itemsCount, Guid? customerId = null)
    {
        var customer = _fixture.Create<Customer>();
        if (customerId.HasValue)
            customer.Id = customerId.Value;

        return new SaleProcessingData
        {
            Customer = customer,
            OriginalAmount = amount,
            ItemsCount = itemsCount,
            Products = new List<ProductProcessingData>()
        };
    }

    [Fact]
    public async Task ApplyRuleAsync_WhenDiscountNotUsedThisMonth_ShouldApply30PercentDiscount()
    {
        // Arrange
        var data = CreateValidSaleData(100m, 5);
        _customerRepositoryMock
            .Setup(x => x.HasUsedInternDiscountThisMonthAsync(data.Customer.Id))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.DiscountPercentage.Should().Be(30m);
        result.FinalAmount.Should().Be(70m); // 100 - 30%
        result.RuleApplied.Should().Be("InternRule");
    }

    [Fact]
    public async Task ApplyRuleAsync_WhenDiscountAlreadyUsedThisMonth_ShouldApplyNoDiscount()
    {
        // Arrange
        var data = CreateValidSaleData(100m, 5);
        _customerRepositoryMock
            .Setup(x => x.HasUsedInternDiscountThisMonthAsync(data.Customer.Id))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.DiscountPercentage.Should().Be(0m);
        result.FinalAmount.Should().Be(100m);
    }

    [Fact]
    public async Task ApplyRuleAsync_WhenDiscountApplied_ShouldMarkDiscountAsUsed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var data = CreateValidSaleData(100m, 5, customerId);
        _customerRepositoryMock
            .Setup(x => x.HasUsedInternDiscountThisMonthAsync(customerId))
            .ReturnsAsync(false);

        // Act
        await _sut.ApplyRuleAsync(data);

        // Assert
        _customerRepositoryMock.Verify(
            x => x.MarkInternDiscountUsedAsync(customerId),
            Times.Once);
    }

    [Fact]
    public async Task ApplyRuleAsync_WhenNoDiscountApplied_ShouldNotMarkDiscountAsUsed()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var data = CreateValidSaleData(100m, 5, customerId);
        _customerRepositoryMock
            .Setup(x => x.HasUsedInternDiscountThisMonthAsync(customerId))
            .ReturnsAsync(true);

        // Act
        await _sut.ApplyRuleAsync(data);

        // Assert
        _customerRepositoryMock.Verify(
            x => x.MarkInternDiscountUsedAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task ApplyRuleAsync_WithInvalidAmount_ShouldReturnInvalid()
    {
        // Arrange
        var data = CreateValidSaleData(-10m, 5);

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
        var data = CreateValidSaleData(100m, 16); // InternRule allows max 15 items

        // Act
        var result = await _sut.ApplyRuleAsync(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ValidationError.Should().Contain("Items");
    }

    [Fact]
    public void RuleName_ShouldReturnInternRule()
    {
        _sut.RuleName.Should().Be("InternRule");
    }
}
