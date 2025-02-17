using Refactoring.Domain.Enums;
using Refactoring.Domain.Models.Products;

namespace Refactoring.Application.Services.Payments;

internal class PaymentProcessor(IPaymentStrategyFactory factory) : IPaymentProcessor
{
    public async Task ProcessAsync(Product product, PaymentType paymentType, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(product);
        
        var strategy = factory.GetStrategy(paymentType);
        
        await strategy.ProcessPaymentAsync(product, cancellationToken);
    }
}