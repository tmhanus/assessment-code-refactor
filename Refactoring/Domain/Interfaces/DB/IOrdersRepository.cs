using Refactoring.Domain.Models.Products;

namespace Refactoring.Domain.Interfaces.DB;

public interface IProductsRepository
{
    Task<Product?> GetFirstByIdAndTypeAsync(long id, string type, CancellationToken cancellationToken); //TODO I'd use different ID to communicate with outside world
}