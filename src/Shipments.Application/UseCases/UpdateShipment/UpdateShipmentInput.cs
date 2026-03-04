using Shipments.Domain.Models;

namespace Shipments.Application.UseCases.UpdateShipment;

/// <summary>
/// Input data for updating an existing shipment.
/// </summary>
public class UpdateShipmentInput
{
    /// <summary>
    /// Unique identifier of the shipment to update.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Name or description of the package.
    /// </summary>
    public string? PackageName { get; set; }

    /// <summary>
    /// Weight of the package.
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Dimensions of the package.
    /// </summary>
    public Dimensions? Dimensions { get; set; }

    /// <summary>
    /// Cost of shipping the package.
    /// </summary>
    public decimal? ShippingCost { get; set; }

    /// <summary>
    /// Destination address for the shipment.
    /// </summary>
    public string? DestinationAddress { get; set; }

    /// <summary>
    /// Name of the user updating the shipment (extracted from header).
    /// </summary>
    public string? Creator { get; set; }

    /// <summary>
    /// New status for the shipment (optional). Valid values: pending, in_transit, delivered, cancelled.
    /// </summary>
    public string? Status { get; set; }
}
