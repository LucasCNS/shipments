using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shipments.Application.Repositories;
using Shipments.Domain.Models;

namespace Shipments.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core implementation of the shipment repository.
/// </summary>
public class ShipmentEFRepository : IShipmentRepository
{
    private readonly ShipmentsDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the ShipmentEFRepository class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public ShipmentEFRepository(ShipmentsDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Creates a new shipment and saves it to the database.
    /// </summary>
    /// <param name="shipment">The shipment to create.</param>
    /// <returns>The created shipment.</returns>
    public async Task<Shipment> CreateAsync(Shipment shipment)
    {
        if (shipment == null)
            throw new ArgumentNullException(nameof(shipment));

        _dbContext.Shipments.Add(shipment);
        await _dbContext.SaveChangesAsync();
        return shipment;
    }

    /// <summary>
    /// Retrieves a shipment by its unique identifier.
    /// </summary>
    /// <param name="id">The shipment identifier.</param>
    /// <returns>The shipment if found; null otherwise.</returns>
    public async Task<Shipment?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Shipments.FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Retrieves all shipments with optional filtering and pagination.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="offset">Number of records to skip.</param>
    /// <param name="limit">Number of records to return.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A collection of shipments matching the criteria.</returns>
    public async Task<IEnumerable<Shipment>> GetAllAsync(
        string? status = null,
        int offset = 0,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Shipments.AsQueryable();

        // Apply status filter if provided
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(s => s.Status == status);
        }

        // Apply pagination
        var shipments = await query
            .OrderByDescending(s => s.DateCreated)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return shipments;
    }

    /// <summary>
    /// Gets the total count of shipments matching the optional status filter.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The total count of shipments matching the filter.</returns>
    public async Task<int> GetCountAsync(
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Shipments.AsQueryable();

        // Apply status filter if provided
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(s => s.Status == status);
        }

        return await query.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing shipment.
    /// </summary>
    /// <param name="shipment">The shipment with updated data.</param>
    /// <returns>The updated shipment if successful; null if shipment not found.</returns>
    public async Task<Shipment?> UpdateAsync(Shipment shipment)
    {
        if (shipment == null)
            throw new ArgumentNullException(nameof(shipment));

        // Check if shipment exists
        var existingShipment = await _dbContext.Shipments.FirstOrDefaultAsync(s => s.Id == shipment.Id);
        if (existingShipment == null)
            return null;

        // Update properties
        existingShipment.PackageName = shipment.PackageName;
        existingShipment.Weight = shipment.Weight;
        existingShipment.Dimensions = shipment.Dimensions;
        existingShipment.ShippingCost = shipment.ShippingCost;
        existingShipment.OriginZipCode = shipment.OriginZipCode;
        existingShipment.DestinationZipCode = shipment.DestinationZipCode;
        existingShipment.DestinationAddress = shipment.DestinationAddress;
        existingShipment.DateLastUpdated = shipment.DateLastUpdated;
        existingShipment.Status = shipment.Status;

        _dbContext.Shipments.Update(existingShipment);
        await _dbContext.SaveChangesAsync();

        return existingShipment;
    }
}
