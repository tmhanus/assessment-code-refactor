using Refactoring.Domain.Enums;

namespace Refactoring.Application.Services.Payments;

public interface IPaymentStrategyFactory
{
    IPaymentStrategy GetStrategy(PaymentType paymentType);
}