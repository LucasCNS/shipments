using System;

namespace Costs.Domain.Models;

/// <summary>
/// Represents a shipping cost calculation record.
/// </summary>
public class ShippingCost
{
    public Guid Id { get; set; }
    public string OriginZipCode { get; set; } = string.Empty;
    public string DestinationZipCode { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal Volume { get; set; }
    public bool IsExpress { get; set; }
    public decimal BaseCost { get; set; }
    public decimal FinalCost { get; set; }
    public DateTime CreatedAt { get; set; }
}
