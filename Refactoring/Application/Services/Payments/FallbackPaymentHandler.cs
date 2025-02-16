using Refactoring.Domain.Models.Products;

namespace Refactoring.Application.Services.Payments;

internal class FallbackPaymentHandler : IPaymentHandler
{   
    public Task HandlePaymentAsync(Product product, string paymentType, CancellationToken cancellationToken)
    {
        throw new Exception("Unsupported payment type");
    }

    public void SetNextHandler(IPaymentHandler paymentHandler)
    {
        throw new NotSupportedException("You can't set the next handler for the fallback handler");
    }
}