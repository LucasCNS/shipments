using System.Collections.Generic;
using Shipments.Domain.Models;
using Shipments.Domain.Results;

namespace Shipments.Application.UseCases.ListShipments;

/// <summary>
/// Output data from the list shipments use case.
/// </summary>
public class ListShipmentsOutput
{
    /// <summary>
    /// Total count of shipments matching the filter (without pagination).
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Number of records skipped (pagination offset).
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Number of records returned in this response.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// List of shipments matching the filter and pagination criteria.
    /// </summary>
    public List<Shipment> Results { get; set; } = new();

    /// <summary>
    /// Error information if the operation failed; null if successful.
    /// </summary>
    public Error? Error { get; set; }
}
