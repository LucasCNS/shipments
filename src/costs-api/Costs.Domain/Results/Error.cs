using System.Collections.Generic;

namespace Costs.Domain.Results;

/// <summary>
/// Represents an error that occurred during business logic execution.
/// </summary>
public class Error
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int CorrespondingStatusCode { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}
