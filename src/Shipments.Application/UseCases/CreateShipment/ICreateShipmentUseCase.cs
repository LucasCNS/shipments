using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipments.Application.UseCases.CreateShipment;

/// <summary>
/// Use case interface for creating a new shipment.
/// </summary>
public interface ICreateShipmentUseCase
{
    /// <summary>
    /// Handles the creation of a new shipment.
    /// </summary>
    /// <param name="input">The input data for creating a shipment.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The shipment creation output.</returns>
    Task<CreateShipmentOutput> HandleAsync(CreateShipmentInput input, CancellationToken cancellationToken);
}
