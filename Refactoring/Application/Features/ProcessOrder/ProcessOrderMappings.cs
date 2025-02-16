namespace Refactoring.Application.Features.ProcessOrder;

public static class ProcessOrderMappings
{
    public static ProcessOrderCommand DtoToCommand(ProcessOrderDto dto) =>
        new()
        {
            PaymentType = dto.PaymentType, 
            ProductId = dto.ProductId, 
            ProductType = dto.ProductType,
        };
}