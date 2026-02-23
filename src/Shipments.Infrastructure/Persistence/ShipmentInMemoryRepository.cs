using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shipments.Application.Repositories;
using Shipments.Domain.Models;

namespace Shipments.Infrastructure.Persistence;

/// <summary>
/// In-memory implementation of the shipment repository.
/// Thread-safe using lock for concurrent access.
/// </summary>
public class ShipmentInMemoryRepository : IShipmentRepository
{
    private static readonly List<Shipment> _shipments = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Creates a new shipment and stores it in memory.
    /// </summary>
    /// <param name="shipment">The shipment to create.</param>
    /// <returns>The created shipment.</returns>
    public Task<Shipment> CreateAsync(Shipment shipment)
    {
        lock (_lock)
        {
            _shipments.Add(shipment);
            return Task.FromResult(shipment);
        }
    }

    /// <summary>
    /// Retrieves a shipment by its unique identifier.
    /// </summary>
    /// <param name="id">The shipment identifier.</param>
    /// <returns>The shipment if found; null otherwise.</returns>
    public Task<Shipment?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            var shipment = _shipments.Find(s => s.Id == id);
            return Task.FromResult(shipment);
        }
    }

    /// <summary>
    /// Retrieves all shipments with optional filtering and pagination.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="offset">Number of records to skip.</param>
    /// <param name="limit">Number of records to return.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A collection of shipments matching the criteria.</returns>
    public Task<IEnumerable<Shipment>> GetAllAsync(string? status = null, int offset = 0, int limit = 10, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _shipments.AsEnumerable();

            // Apply status filter if provided
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(s => s.Status == status);
            }

            // Apply pagination: skip offset records, then take limit records
            var result = query.Skip(offset).Take(limit).ToList();
            return Task.FromResult(result.AsEnumerable());
        }
    }

    /// <summary>
    /// Gets the total count of shipments matching the optional status filter.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The total count of shipments matching the filter.</returns>
    public Task<int> GetCountAsync(string? status = null, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _shipments.AsEnumerable();

            // Apply status filter if provided
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(s => s.Status == status);
            }

            var count = query.Count();
            return Task.FromResult(count);
        }
    }

    /// <summary>
    /// Gets all shipments stored in memory.
    /// For internal use and testing purposes.
    /// </summary>
    /// <returns>A copy of the internal shipments list.</returns>
    internal static List<Shipment> GetAllShipments()
    {
        lock (_lock)
        {
            return new List<Shipment>(_shipments);
        }
    }

    /// <summary>
    /// Clears all shipments from memory.
    /// For testing purposes only.
    /// </summary>
    internal static void ClearAll()
    {
        lock (_lock)
        {
            _shipments.Clear();
        }
    }
}
