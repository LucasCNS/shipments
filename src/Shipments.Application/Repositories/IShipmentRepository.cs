using System.Threading.Tasks;
using Shipments.Domain.Models;

namespace Shipments.Application.Repositories;

/// <summary>
/// Repository interface for Shipment persistence operations.
/// </summary>
public interface IShipmentRepository
{
    /// <summary>
    /// Creates a new shipment.
    /// </summary>
    /// <param name="shipment">The shipment to create.</param>
    /// <returns>The created shipment.</returns>
    Task<Shipment> CreateAsync(Shipment shipment);
}
