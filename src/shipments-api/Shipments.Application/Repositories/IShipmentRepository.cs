using System;
using System.Collections.Generic;
using System.Threading;
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

    /// <summary>
    /// Retrieves a shipment by its unique identifier.
    /// </summary>
    /// <param name="id">The shipment identifier.</param>
    /// <returns>The shipment if found; null otherwise.</returns>
    Task<Shipment?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all shipments with optional filtering and pagination.
    /// </summary>
    /// <param name="status">Optional status filter (e.g., "pending", "in_transit", "delivered", "cancelled").</param>
    /// <param name="offset">Number of records to skip (default: 0).</param>
    /// <param name="limit">Number of records to return (default: 10, max: 100).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A collection of shipments matching the criteria.</returns>
    Task<IEnumerable<Shipment>> GetAllAsync(string? status = null, int offset = 0, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of shipments matching the optional status filter.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The total count of shipments matching the filter.</returns>
    Task<int> GetCountAsync(string? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing shipment.
    /// </summary>
    /// <param name="shipment">The shipment with updated data.</param>
    /// <returns>The updated shipment if successful; null if shipment not found.</returns>
    Task<Shipment?> UpdateAsync(Shipment shipment);
}
