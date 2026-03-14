using System.Collections.Generic;

namespace Shipments.Domain.Results;

/// <summary>
/// Represents an error that occurred during business logic execution.
/// </summary>
public class Error
{
    /// <summary>
    /// Error code (e.g., "INVALID_WEIGHT", "EMPTY_PACKAGE_NAME").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The HTTP status code corresponding to this error.
    /// </summary>
    public int CorrespondingStatusCode { get; set; }

    /// <summary>
    /// List of validation error messages (when multiple validation errors occur).
    /// </summary>
    public List<string> ValidationErrors { get; set; } = new();
}
