using System.Threading;
using System.Threading.Tasks;

namespace Shipments.Application.UseCases.ListShipments;

/// <summary>
/// Use case interface for listing shipments.
/// </summary>
public interface IListShipmentsUseCase
{
    /// <summary>
    /// Handles the retrieval of a list of shipments with optional filtering and pagination.
    /// </summary>
    /// <param name="input">The input data for listing shipments.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The shipments list output.</returns>
    Task<ListShipmentsOutput> HandleAsync(ListShipmentsInput input, CancellationToken cancellationToken);
}
