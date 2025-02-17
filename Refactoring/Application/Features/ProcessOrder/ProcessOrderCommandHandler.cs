using MediatR;
using Microsoft.Extensions.Options;
using Refactoring.Application.Models.Settings;
using Refactoring.Application.Services.Payments;
using Refactoring.Domain.Enums;
using Refactoring.Domain.Exceptions;
using Refactoring.Domain.Interfaces;
using Refactoring.Domain.Interfaces.DB;

namespace Refactoring.Application.Features.ProcessOrder;

public class ProcessOrderCommandHandler(
    IPaymentProcessor paymentProcessor,
    IProductsRepository productsRepository,
    IEmailClient emailClient, 
    IOptionsSnapshot<EmailSettings> emailOptions, 
    IOutputWriter outputWriter,
    ILogger<ProcessOrderCommandHandler> logger) : IRequestHandler<ProcessOrderCommand>
{
    private const string HardcodedFakeEmail = "user@example.com";
    private const string HardcodedFakeEmailMessage = "Order confirmed";
    
    public async Task Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        var paymentType = PaymentType.FromString(request.PaymentType);
        if (paymentType == null)
        {
            throw new UnknownPaymentTypeException(request.PaymentType);
        }
        
        var product = await productsRepository.GetFirstByIdAndTypeAsync(request.ProductId, request.ProductType, cancellationToken);
        if (product == null)
        {
            throw new NotFoundException("Product not found");
        }
        
        outputWriter.Write($"Product {product.Name} fetched."); // Comment - This is here to keep existing functionality 
        
        try
        {
            await paymentProcessor.ProcessAsync(product, paymentType, cancellationToken);
        }
        catch (NotSupportedPaymentTypeException ex)
        {
            logger.LogError(
                ex, 
                "Payment type {PaymentType} is not supported for product {@Product}",
                request.PaymentType, 
                product);
            
            throw;
        }
        
        await emailClient.SendEmailAsync(
            emailOptions.Value.OutgoingUrl, 
            HardcodedFakeEmail,
            HardcodedFakeEmailMessage,
            cancellationToken);
    }
}