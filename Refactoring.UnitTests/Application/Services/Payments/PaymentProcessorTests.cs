using Moq;
using Refactoring.Application.Services.Payments;
using Refactoring.Domain.Enums;
using Refactoring.Domain.Exceptions;
using Refactoring.Domain.Models.Products;
using Shouldly;

namespace Refactoring.UnitTests.Application.Services.Payments;

public class PaymentProcessorTests
{
    private readonly Mock<IPaymentStrategyFactory> _paymentStrategyFactoryMock;
    private readonly Mock<IPaymentStrategy> _payPalPaymentStrategyMock;
    private readonly PaymentProcessor _sut;

    public PaymentProcessorTests()
    {
        _paymentStrategyFactoryMock = new Mock<IPaymentStrategyFactory>();
        _payPalPaymentStrategyMock = new Mock<IPaymentStrategy>();
        
        _sut = new PaymentProcessor(_paymentStrategyFactoryMock.Object);
    }

    [Fact]
    public async Task ProcessAsync_ShouldCallCorrectStrategy_WhenPaymentTypeIsValid()
    {
        // Arrange
        var product = new Product { Id = 1, Price = 100.0m, Name = "Test Product", Type = "Type1" };
        var expectedPaymentType = PaymentType.PayPal;
        var notExpectedPaymentType = PaymentType.CreditCard;
        var creditCardPaymentStrategyMock = new Mock<IPaymentStrategy>();
        
        _paymentStrategyFactoryMock
            .Setup(factory => factory.GetStrategy(expectedPaymentType))
            .Returns(_payPalPaymentStrategyMock.Object);
        
        _paymentStrategyFactoryMock
            .Setup(factory => factory.GetStrategy(notExpectedPaymentType))
            .Returns(creditCardPaymentStrategyMock.Object);
        
        _payPalPaymentStrategyMock
            .Setup(strategy => strategy.ProcessPaymentAsync(product, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.ProcessAsync(product, expectedPaymentType, CancellationToken.None);

        // Assert
        _payPalPaymentStrategyMock.Verify(strategy => strategy.ProcessPaymentAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        creditCardPaymentStrategyMock.Verify(strategy => strategy.ProcessPaymentAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_ShouldThrowException_WhenStrategyIsNotSupported()
    {
        // Arrange
        var product = new Product { Id = 2, Price = 200.0m, Name = "Another Product", Type = "Type2" };
        var notSupportedPaymentType = PaymentType.PayPal;
        
        _paymentStrategyFactoryMock
            .Setup(factory => factory.GetStrategy(notSupportedPaymentType))
            .Throws(new NotSupportedPaymentTypeException("Invalid payment type"));

        // Act & Assert
        await Should.ThrowAsync<NotSupportedPaymentTypeException>(async () =>
            await _sut.ProcessAsync(product, notSupportedPaymentType, CancellationToken.None));
    }
}