namespace Costs.Domain.Models;

/// <summary>
/// Represents the physical dimensions of a package.
/// </summary>
public class Dimensions
{
    /// <summary>Length in centimetres.</summary>
    public decimal Length { get; set; }

    /// <summary>Width in centimetres.</summary>
    public decimal Width { get; set; }

    /// <summary>Height in centimetres.</summary>
    public decimal Height { get; set; }

    /// <summary>Volume in cubic centimetres.</summary>
    public decimal Volume => Length * Width * Height;
}
