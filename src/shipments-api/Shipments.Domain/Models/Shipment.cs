using System;

namespace Shipments.Domain.Models;

/// <summary>
/// Represents a shipment entity in the system.
/// </summary>
public class Shipment
{
    /// <summary>
    /// Unique identifier for the shipment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name or package name of the shipment.
    /// </summary>
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    /// Weight of the package.
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Dimensions of the package (length, width, height).
    /// </summary>
    public Dimensions? Dimensions { get; set; }

    /// <summary>
    /// Cost of shipping the package.
    /// </summary>
    public decimal ShippingCost { get; set; }

    /// <summary>
    /// Destination address for the shipment.
    /// </summary>
    public string DestinationAddress { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the shipment was created.
    /// </summary>
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Date and time of the last update to the shipment.
    /// </summary>
    public DateTime DateLastUpdated { get; set; }

    /// <summary>
    /// Name of the user who created the shipment.
    /// </summary>
    public string Creator { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the shipment (e.g., "pending", "shipped", "delivered").
    /// </summary>
    public string Status { get; set; } = "pending";
}
