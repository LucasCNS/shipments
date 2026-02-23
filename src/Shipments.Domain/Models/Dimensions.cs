namespace Shipments.Domain.Models;

/// <summary>
/// Represents the dimensions of a package.
/// </summary>
public class Dimensions
{
    /// <summary>
    /// Length of the package.
    /// </summary>
    public decimal Length { get; set; }

    /// <summary>
    /// Width of the package.
    /// </summary>
    public decimal Width { get; set; }

    /// <summary>
    /// Height of the package.
    /// </summary>
    public decimal Height { get; set; }
}
