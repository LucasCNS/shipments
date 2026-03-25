namespace Costs.Application.DTOs;

/// <summary>
/// Request DTO for calculating shipping cost.
/// </summary>
public class CalculateShippingCostRequest
{
    public string? OriginZipCode { get; set; }
    public string? DestinationZipCode { get; set; }
    public decimal Weight { get; set; }
    public DimensionsDto? Dimensions { get; set; }
    public bool IsExpress { get; set; }
}

/// <summary>
/// Package dimensions in centimetres.
/// </summary>
public class DimensionsDto
{
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
}
