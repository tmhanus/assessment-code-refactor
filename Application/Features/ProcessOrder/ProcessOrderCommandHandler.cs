using MediatR;
using Microsoft.Extensions.Options;
using Refactoring.Application.Models.Settings;
using Refactoring.Application.Services.Payments;
using Refactoring.Domain.Interfaces;
using Refactoring.Domain.Interfaces.DB;

namespace Refactoring.Application.Features.ProcessOrder;

public class ProcessOrderCommandHandler(
    Func<IPaymentHandler> getPaymentHandler,
    IProductsRepository productsRepository,
    IEmailClient emailClient, 
    IOptionsSnapshot<EmailSettings> emailOptions, 
    IOutputWriter outputWriter) : IRequestHandler<ProcessOrderCommand>
{
    private const string HardcodedFakeEmail = "user@example.com";
    private const string HardcodedFakeEmailMessage = "Order confirmed";
    
    public async Task Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetFirstByIdAndTypeAsync(request.ProductId, request.ProductType, cancellationToken);
        if (product == null)
        {
            throw new Exception("Product not found");
        }
        
        outputWriter.Write($"Product {product.Name} fetched.");
        
        var payment = getPaymentHandler();
        await payment.HandlePaymentAsync(product, request.PaymentType, cancellationToken);

        await emailClient.SendEmailAsync(
            emailOptions.Value.OutgoingUrl, 
            HardcodedFakeEmail,
            HardcodedFakeEmailMessage,
            cancellationToken);
    }
}