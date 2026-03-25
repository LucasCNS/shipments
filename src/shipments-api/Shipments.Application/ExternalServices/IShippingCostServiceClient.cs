using System.Threading;
using System.Threading.Tasks;
using Shipments.Domain.Models;

namespace Shipments.Application.ExternalServices;

/// <summary>
/// Client interface for calling the external Costs API to calculate shipping cost.
/// Returns null if the service is unavailable.
/// </summary>
public interface IShippingCostServiceClient
{
    Task<decimal?> CalculateShippingCostAsync(
        string originZipCode,
        string destinationZipCode,
        decimal weight,
        Dimensions dimensions,
        CancellationToken cancellationToken = default);
}
