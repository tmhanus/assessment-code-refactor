using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Refactoring.Application.Features.ProcessOrder;
using Refactoring.Application.Models.Settings;
using Refactoring.Application.Services.Payments;
using Refactoring.Domain.Enums;
using Refactoring.Domain.Exceptions;
using Refactoring.Domain.Interfaces;
using Refactoring.Domain.Interfaces.DB;
using Refactoring.Domain.Models.Products;
using Shouldly;

namespace Refactoring.UnitTests.Application.Features;

public class ProcessOrderCommandHandlerTests
{
    private const string FakeEmailUrl = "https://fake-email.com";
    private const string HardcodedUserEmail = "user@example.com";
    private const string HardcodedMessage = "Order confirmed";
    
    private readonly Mock<IPaymentProcessor> _paymentProcessorMock = new();
    private readonly Mock<IProductsRepository> _productsRepositoryMock = new();
    private readonly Mock<IEmailClient> _emailClientMock = new();
    private readonly Mock<IOptionsSnapshot<EmailSettings>> _emailOptionsMock = new();
    private readonly Mock<IOutputWriter> _outputWriterMock = new();
    
    private readonly ProcessOrderCommandHandler _handler;
    
    public ProcessOrderCommandHandlerTests()
    {
        _emailOptionsMock.Setup(e => e.Value).Returns(new EmailSettings { OutgoingUrl = FakeEmailUrl });
        
        _handler = new ProcessOrderCommandHandler(
            _paymentProcessorMock.Object,
            _productsRepositoryMock.Object,
            _emailClientMock.Object,
            _emailOptionsMock.Object,
            _outputWriterMock.Object,
            new NullLogger<ProcessOrderCommandHandler>());
    }

    [Fact]
    public async Task Handle_ThrowsUnknownPaymentTypeException_WhenPaymentTypeIsInvalid()
    {
        // Arrange
        var command = new ProcessOrderCommand { PaymentType = "InvalidType", ProductId = 1, ProductType = "TypeA" };
        
        // Act & Assert
        await Should.ThrowAsync<UnknownPaymentTypeException>(async () => 
            await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenProductIsNotFound()
    {
        // Arrange
        var command = new ProcessOrderCommand
        {
            PaymentType = PaymentType.CreditCard.ToString(), 
            ProductId = 1, 
            ProductType = "TypeA"
        };
        
        _productsRepositoryMock
            .Setup(p => p.GetFirstByIdAndTypeAsync(
                command.ProductId, 
                command.ProductType, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product)null);
        
        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () => 
            await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ProcessesPayment_WhenProductExists()
    {
        // Arrange
        var product = new Product
        {
            Name = "Sample Product", 
            Id = 1, 
            Type = "TypeA", 
            Price = 1.0m
        };
        
        var command = new ProcessOrderCommand
        {
            PaymentType = PaymentType.CreditCard, 
            ProductId = product.Id, 
            ProductType = product.Type
        };
        
        _productsRepositoryMock
            .Setup(p => p.GetFirstByIdAndTypeAsync(
                product.Id,
                product.Type,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        _paymentProcessorMock
            .Setup(p => p.ProcessAsync(
                product, 
                PaymentType.CreditCard,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        _paymentProcessorMock.Verify(p => p.ProcessAsync(
            product, 
            PaymentType.CreditCard, 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenPaymentProcessingFails()
    {
        // Arrange
        var product = new Product { Name = "Sample Product", Id = 1, Type = "TypeA", Price = 1.0m };
        var command = new ProcessOrderCommand { PaymentType = PaymentType.PayPal, ProductId = product.Id, ProductType = product.Type };
        
        _productsRepositoryMock
            .Setup(p => p.GetFirstByIdAndTypeAsync(
                product.Id, 
                product.Type, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        _paymentProcessorMock
            .Setup(p => p.ProcessAsync(
                product, 
                PaymentType.PayPal,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotSupportedPaymentTypeException("Not Lucky Today"));
        
        // Act & Assert
        await Should.ThrowAsync<NotSupportedPaymentTypeException>(async () => 
            await _handler.Handle(command, CancellationToken.None));
        
        _emailClientMock.Verify(x => x.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_SendsEmail_WhenPaymentSucceeds()
    {
        // Arrange
        var product = new Product { Name = "Sample Product", Id = 1, Type = "TypeA", Price = 1.0m };
        var command = new ProcessOrderCommand { PaymentType = PaymentType.CreditCard, ProductId = product.Id, ProductType = product.Type };
        
        _productsRepositoryMock
            .Setup(p => p.GetFirstByIdAndTypeAsync(product.Id,product.Type, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        _paymentProcessorMock.Setup(p => p.ProcessAsync(product, It.IsAny<PaymentType>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        _emailClientMock.Verify(e => e
            .SendEmailAsync(
                FakeEmailUrl, 
                HardcodedUserEmail, 
                HardcodedMessage, 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}