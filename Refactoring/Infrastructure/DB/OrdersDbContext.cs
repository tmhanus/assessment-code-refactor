using Microsoft.EntityFrameworkCore;
using Refactoring.Domain.Models.Products;

namespace Refactoring.Infrastructure.DB;

internal class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    /// <summary>
    /// TODO Improve separation of concerns - could introduce Infrastructure level ProductEntity and Map it to
    /// domain object once retrieved from DB
    /// </summary>
    public DbSet<Product> Products { get; set; } 
}