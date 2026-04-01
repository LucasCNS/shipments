using System.Threading;
using System.Threading.Tasks;
using Shipments.Application.Results;

namespace Shipments.Application.UseCases.GetShipmentById;

/// <summary>
/// Use case interface for retrieving a shipment by its ID.
/// </summary>
public interface IGetShipmentByIdUseCase
{
    /// <summary>
    /// Handles the retrieval of a shipment by its unique identifier.
    /// </summary>
    /// <param name="input">The input data containing the shipment ID.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The shipment retrieval output.</returns>
    Task<Result<GetShipmentByIdOutput>> HandleAsync(GetShipmentByIdInput input, CancellationToken cancellationToken);
}
