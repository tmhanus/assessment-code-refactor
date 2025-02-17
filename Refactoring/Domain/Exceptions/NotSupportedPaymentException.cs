namespace Refactoring.Domain.Exceptions;

public class NotSupportedPaymentTypeException(string paymentType) : Exception($"Not supported payment type - {paymentType}");