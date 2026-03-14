namespace Shipments.Application.UseCases.UpdateShipmentStatus;

/// <summary>
/// Input data for updating a shipment status.
/// </summary>
public class UpdateShipmentStatusInput
{
    /// <summary>
    /// The shipment ID (UUID).
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The new status for the shipment (one of: pending, in_transit, delivered, cancelled).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Name of the user updating the shipment (extracted from header).
    /// </summary>
    public string? Creator { get; set; }
}
