using Microsoft.Extensions.DependencyInjection;
using Shipments.Application.UseCases.CreateShipment;
using Shipments.Application.UseCases.GetShipmentById;
using Shipments.Application.UseCases.ListShipments;
using Shipments.Application.UseCases.UpdateShipment;

namespace Shipments.Application;

/// <summary>
/// Dependency injection configuration for Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateShipmentUseCase, CreateShipmentUseCase>();
        services.AddScoped<IGetShipmentByIdUseCase, GetShipmentByIdUseCase>();
        services.AddScoped<IListShipmentsUseCase, ListShipmentsUseCase>();
        services.AddScoped<IUpdateShipmentUseCase, UpdateShipmentUseCase>();
        return services;
    }
}
