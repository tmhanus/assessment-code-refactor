using MediatR;

namespace Refactoring.Application.Features.ProcessOrder;

public record ProcessOrderCommand : IRequest
{
    public required long ProductId { get; init; }
    public required string ProductType { get; init; }
    public required string PaymentType { get; init; }
}