namespace Refactoring.Domain.Exceptions;

public class NotFoundException(string msg) : Exception(msg);