using System.Threading;
using System.Threading.Tasks;
using Shipments.Application.Results;

namespace Shipments.Application.UseCases.UpdateShipment;

/// <summary>
/// Use case interface for updating an existing shipment.
/// </summary>
public interface IUpdateShipmentUseCase
{
    /// <summary>
    /// Handles the update of an existing shipment.
    /// </summary>
    /// <param name="input">The input data for updating a shipment.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The shipment update output.</returns>
    Task<Result<UpdateShipmentOutput>> HandleAsync(UpdateShipmentInput input, CancellationToken cancellationToken);
}
