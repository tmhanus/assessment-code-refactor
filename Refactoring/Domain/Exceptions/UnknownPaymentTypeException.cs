namespace Refactoring.Domain.Exceptions;

public class UnknownPaymentTypeException(string paymentType) : Exception($"Unknown payment type - {paymentType}");