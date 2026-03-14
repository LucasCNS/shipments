using Shipments.Domain.Models;

namespace Shipments.Application.UseCases.CreateShipment;

/// <summary>
/// Input data for creating a new shipment.
/// </summary>
public class CreateShipmentInput
{
    /// <summary>
    /// Name or description of the package.
    /// </summary>
    public string? PackageName { get; set; }

    /// <summary>
    /// Weight of the package.
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Dimensions of the package.
    /// </summary>
    public Dimensions? Dimensions { get; set; }

    /// <summary>
    /// Cost of shipping the package.
    /// </summary>
    public decimal ShippingCost { get; set; }

    /// <summary>
    /// Destination address for the shipment.
    /// </summary>
    public string? DestinationAddress { get; set; }

    /// <summary>
    /// Name of the user creating the shipment (extracted from header).
    /// </summary>
    public string? Creator { get; set; }
}
