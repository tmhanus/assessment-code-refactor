using Refactoring.Domain.Models.Products;

namespace Refactoring.Application.Services.Payments;

public interface IPaymentHandler
{
    Task HandlePaymentAsync(Product product, string paymentType, CancellationToken cancellationToken);
    
    void SetNextHandler(IPaymentHandler paymentHandler);
}