using System;
using System.Text.Json.Serialization;
using Shipments.Domain.Models;
using Shipments.Domain.Results;

namespace Shipments.Application.UseCases.UpdateShipment;

/// <summary>
/// Output data from the update shipment use case.
/// </summary>
public class UpdateShipmentOutput
{
    /// <summary>
    /// Unique identifier for the shipment.
    /// </summary>
    public Guid Id { get; set; }

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
    public string? Creator { get; set; }

    /// <summary>
    /// Current status of the shipment.
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// Error information if the operation failed; null if successful.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Error? Error { get; set; }
}
