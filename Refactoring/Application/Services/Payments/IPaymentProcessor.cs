using Refactoring.Domain.Enums;
using Refactoring.Domain.Models.Products;

namespace Refactoring.Application.Services.Payments;

public interface IPaymentProcessor
{
    Task ProcessAsync(Product product, PaymentType paymentType, CancellationToken cancellationToken);
}