namespace Refactoring.Application.Features.ProcessOrder;

public record ProcessOrderDto
{
    public required int ProductId { get; init; }
    public required string ProductType { get; init; }
    public required string PaymentType { get; init; }
}