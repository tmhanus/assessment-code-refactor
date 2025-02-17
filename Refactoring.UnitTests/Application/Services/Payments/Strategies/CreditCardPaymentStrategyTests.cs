using Moq;
using Refactoring.Application.Services.Payments.Strategies;
using Refactoring.Domain.Interfaces;
using Refactoring.Domain.Models.Products;

namespace Refactoring.UnitTests.Application.Services.Payments.Strategies;

public class CreditCardPaymentStrategyTests
{
    private readonly Mock<IOutputWriter> _outputWriterMock;
    private readonly CreditCardPaymentStrategy _sut;

    public CreditCardPaymentStrategyTests()
    {
        _outputWriterMock = new Mock<IOutputWriter>();
        
        _sut = new CreditCardPaymentStrategy(_outputWriterMock.Object);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ShouldWriteToOutput_WhenPaymentIsProcessed()
    {
        // Arrange
        var product = new Product { Id = 1, Price = 100.0m, Name = "Test Product", Type = "Type1" };
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.ProcessPaymentAsync(product, cancellationToken);

        // Assert
        _outputWriterMock.Verify(writer => writer.Write("Processing credit card payment for 100.0"), Times.Once);
    }
}