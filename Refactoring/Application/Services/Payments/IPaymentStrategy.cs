using Refactoring.Domain.Enums;
using Refactoring.Domain.Models.Products;

namespace Refactoring.Application.Services.Payments;

public interface IPaymentStrategy
{
    PaymentType PaymentType { get; }
    Task ProcessPaymentAsync(Product product, CancellationToken cancellationToken);
}