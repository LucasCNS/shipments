namespace Shipments.Application.UseCases.ListShipments;

/// <summary>
/// Input data for listing shipments with optional filtering and pagination.
/// </summary>
public class ListShipmentsInput
{
    /// <summary>
    /// Optional status filter (e.g., "pending", "in_transit", "delivered", "cancelled").
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Number of records to skip (default: 0, minimum: 0).
    /// </summary>
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Number of records to return (default: 10, maximum: 100, minimum: 0).
    /// </summary>
    public int Limit { get; set; } = 10;

    /// <summary>
    /// Name of the user requesting the list (extracted from header).
    /// </summary>
    public string? Creator { get; set; }
}
