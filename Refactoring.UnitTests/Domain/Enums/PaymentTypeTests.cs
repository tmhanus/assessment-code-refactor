using Refactoring.Domain.Enums;
using Shouldly;

namespace Refactoring.UnitTests.Domain.Enums;

public class PaymentTypeTests
{
    [Theory]
    [InlineData("PayPal", true)]
    [InlineData("CreditCard", true)]
    [InlineData("InvalidPayment", false)]
    [InlineData("paypal", true)]
    [InlineData("creditcard", true)]
    public void IsValid_ShouldReturnCorrectResult(string name, bool expectedResult)
    {
        // Act
        var result = PaymentType.IsValid(name);

        // Assert
        result.ShouldBe(expectedResult);
    }

    public static IEnumerable<object[]> PaymentTypeTestData =>
        new List<object[]>
        {
            new object[] { "PayPal", PaymentType.PayPal },
            new object[] { "CreditCard", PaymentType.CreditCard }
        };
    
    [Theory]
    [MemberData(nameof(PaymentTypeTestData))]
    public void FromString_ShouldReturnCorrectPaymentType(string name, PaymentType expectedPaymentType)
    {
        // Act
        var result = PaymentType.FromString(name);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedPaymentType); 
    }
    
    [Fact]
    public void FromString_ShouldReturnNull_ForInvalidPaymentType()
    {
        // Act
        var result = PaymentType.FromString("InvalidPayment");

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void FromString_ShouldThrowArgumentException_ForInvalidPaymentTypeExplicitConversion()
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => (PaymentType)"InvalidPayment");
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnCorrectString()
    {
        // Act
        var payPalString = (string)PaymentType.PayPal;
        var creditCardString = (string)PaymentType.CreditCard;

        // Assert
        payPalString.ShouldBe("PayPal");
        creditCardString.ShouldBe("CreditCard");
    }

    [Fact]
    public void ToString_ShouldReturnCorrectString()
    {
        // Act & Assert
        PaymentType.PayPal.ToString().ShouldBe("PayPal");
        PaymentType.CreditCard.ToString().ShouldBe("CreditCard");
    }

    [Fact]
    public void GetAll_ShouldReturnAllPaymentTypes()
    {
        // Act
        var allPaymentTypes = PaymentType.GetAll().ToList();

        // Assert
        allPaymentTypes.ShouldContain(PaymentType.PayPal);
        allPaymentTypes.ShouldContain(PaymentType.CreditCard);
    }
}