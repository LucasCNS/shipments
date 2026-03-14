namespace Shipments.Application.UseCases.GetShipmentById;

/// <summary>
/// Input data for retrieving a shipment by its ID.
/// </summary>
public class GetShipmentByIdInput
{
    /// <summary>
    /// The unique identifier of the shipment to retrieve (UUID format).
    /// </summary>
    public string? ShipmentId { get; set; }
}
