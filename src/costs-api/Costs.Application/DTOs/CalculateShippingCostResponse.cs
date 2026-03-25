using System;

namespace Costs.Application.DTOs;

/// <summary>
/// Response DTO returned after a successful shipping cost calculation.
/// </summary>
public class CalculateShippingCostResponse
{
    public Guid Id { get; set; }
    public string OriginZipCode { get; set; } = string.Empty;
    public string DestinationZipCode { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal Volume { get; set; }
    public bool IsExpress { get; set; }
    public decimal BaseCost { get; set; }
    public decimal FinalCost { get; set; }
    public DateTime CalculatedAt { get; set; }
}
