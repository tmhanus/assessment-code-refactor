using Refactoring.Domain.Enums;
using Refactoring.Domain.Exceptions;

namespace Refactoring.Application.Services.Payments;

internal class PaymentStrategyFactory(IEnumerable<IPaymentStrategy> strategies) : IPaymentStrategyFactory
{
    public IPaymentStrategy GetStrategy(PaymentType paymentType)
    {
        var strategy = strategies.FirstOrDefault(x => x.PaymentType == paymentType);
        
        return strategy ?? throw new NotSupportedPaymentTypeException(paymentType.ToString()); 
    }
}