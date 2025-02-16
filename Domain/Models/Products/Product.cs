namespace Refactoring.Domain.Models.Products;

public record Product
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required string Type { get; init; }
}