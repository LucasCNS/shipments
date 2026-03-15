using Microsoft.EntityFrameworkCore;

namespace Costs.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core DbContext for Costs API.
/// </summary>
public class CostsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CostsDbContext"/> class.
    /// </summary>
    /// <param name="options">The Entity Framework Core database context options.</param>
    public CostsDbContext(DbContextOptions<CostsDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configures the model relationships and constraints.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Add entity configurations here
    }
}
