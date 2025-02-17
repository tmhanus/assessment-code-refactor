using Moq;
using Refactoring.Application.Services.Payments;
using Refactoring.Domain.Enums;
using Refactoring.Domain.Exceptions;
using Shouldly;

namespace Refactoring.UnitTests.Application.Services.Payments;

public class PaymentStrategyFactoryTests
{
    private readonly Mock<IPaymentStrategy> _payPalPaymentStrategyMock;
    private readonly Mock<IPaymentStrategy> _creditCardPaymentStrategyMock;
    private readonly PaymentStrategyFactory _sut;

    public PaymentStrategyFactoryTests()
    {
        _payPalPaymentStrategyMock = new Mock<IPaymentStrategy>();
        _creditCardPaymentStrategyMock = new Mock<IPaymentStrategy>();
        
        _creditCardPaymentStrategyMock.Setup(x => x.PaymentType).Returns(PaymentType.CreditCard);
        _payPalPaymentStrategyMock.Setup(x => x.PaymentType).Returns(PaymentType.PayPal);
        
        _sut = new PaymentStrategyFactory(new List<IPaymentStrategy>
        {
            _payPalPaymentStrategyMock.Object,
            _creditCardPaymentStrategyMock.Object
        });
    }

    [Fact]
    public void GetStrategy_ShouldReturnCorrectStrategy_WhenPaymentTypeIsValid()
    {
        // Arrange
        var expectedPaymentType = PaymentType.PayPal;
    
        // Act
        var result = _sut.GetStrategy(expectedPaymentType);
    
        // Assert
        result.ShouldBe(_payPalPaymentStrategyMock.Object);
    }
    
    [Fact]
    public void GetStrategy_ShouldThrowException_WhenPaymentTypeIsNotSupported()
    {
        // Arrange
        var unsupportedPaymentType = PaymentType.CreditCard;
        
        var sut = new PaymentStrategyFactory(new List<IPaymentStrategy>
        {
            _payPalPaymentStrategyMock.Object,
        });

        // Act & Assert
        Should.Throw<NotSupportedPaymentTypeException>(() => sut.GetStrategy(unsupportedPaymentType));
    }
}