using System;
using System.Collections.Generic;
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
