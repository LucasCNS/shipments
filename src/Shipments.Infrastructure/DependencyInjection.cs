using Microsoft.Extensions.DependencyInjection;
using Shipments.Application.Repositories;
using Shipments.Infrastructure.Persistence;

namespace Shipments.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IShipmentRepository, ShipmentInMemoryRepository>();
        return services;
    }
}
