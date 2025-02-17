using Microsoft.EntityFrameworkCore;
using Refactoring.Domain.Interfaces.DB;
using Refactoring.Domain.Models.Products;

namespace Refactoring.Infrastructure.DB;

internal class ProductsRepository(OrdersDbContext dbContext) : IProductsRepository
{
    public async Task<Product?> GetFirstByIdAndTypeAsync(long id, string type, CancellationToken cancellationToken)
    {
        return await dbContext.Products.FirstOrDefaultAsync(x => 
            x.Id == id && x.Type == type, 
            cancellationToken);
    }
}