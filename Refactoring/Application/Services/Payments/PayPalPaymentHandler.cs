using Refactoring.Domain.Interfaces;
using Refactoring.Domain.Models.Products;

namespace Refactoring.Application.Services.Payments;

internal class PayPalPaymentHandler(IOutputWriter outputWriter) : IPaymentHandler
{
    private IPaymentHandler? _nextHandler;
    
    private const string HandlerType = "PayPal";
    
    public async Task HandlePaymentAsync(Product product, string paymentType, CancellationToken cancellationToken)
    {
        if (paymentType.Equals(HandlerType, StringComparison.OrdinalIgnoreCase))
        {
            await HandleAsync(product, cancellationToken);
            
            return;
        }

        if (_nextHandler != null)
        {
            await _nextHandler.HandlePaymentAsync(product, paymentType, cancellationToken);    
        }
    }

    private Task HandleAsync(Product product, CancellationToken cancellationToken)
    {
        outputWriter.Write($"Processing credit card payment for {product.Price}");
        
        return Task.CompletedTask;
    }

    public void SetNextHandler(IPaymentHandler paymentHandler)
    {
        _nextHandler = paymentHandler;
    }
}