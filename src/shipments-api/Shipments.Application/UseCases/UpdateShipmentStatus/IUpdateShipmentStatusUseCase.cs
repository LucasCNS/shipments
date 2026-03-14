using System.Threading;
using System.Threading.Tasks;
using Shipments.Application.Results;

namespace Shipments.Application.UseCases.UpdateShipmentStatus;

/// <summary>
/// Interface for the UpdateShipmentStatus use case.
/// </summary>
public interface IUpdateShipmentStatusUseCase
{
    /// <summary>
    /// Handles the status update of an existing shipment.
    /// </summary>
    /// <param name="input">The input data containing shipment ID and new status.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A result containing the updated shipment or error information.</returns>
    Task<Result<UpdateShipmentStatusOutput>> HandleAsync(UpdateShipmentStatusInput input, CancellationToken cancellationToken);
}
