using Refactoring.Domain.Enums;
using Refactoring.Domain.Interfaces;
using Refactoring.Domain.Models.Products;

namespace Refactoring.Application.Services.Payments.Strategies;

public class CreditCardPaymentStrategy(IOutputWriter outputWriter) : IPaymentStrategy
{
    public PaymentType PaymentType => PaymentType.CreditCard;
    public Task ProcessPaymentAsync(Product product, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(product);
        
        outputWriter.Write($"Processing credit card payment for {product.Price}");
        
        return Task.CompletedTask;
    }
}