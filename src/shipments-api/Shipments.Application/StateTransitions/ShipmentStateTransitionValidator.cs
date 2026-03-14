using System.Collections.Generic;
using Shipments.Domain.Results;

namespace Shipments.Application.StateTransitions;

/// <summary>
/// Validates shipment status transitions.
/// </summary>
public static class ShipmentStateTransitionValidator
{
    private static readonly HashSet<string> ValidStatuses = new()
    {
        "pending",
        "in_transit",
        "delivered",
        "cancelled"
    };

    private static readonly HashSet<string> FinalStatuses = new()
    {
        "delivered",
        "cancelled"
    };

    private static readonly Dictionary<string, HashSet<string>> AllowedTransitions = new()
    {
        { "pending", new HashSet<string> { "in_transit", "cancelled" } },
        { "in_transit", new HashSet<string> { "delivered", "cancelled" } }
    };

    /// <summary>
    /// Checks if a status value is one of the valid shipment statuses.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <returns>True if the status is valid; false otherwise.</returns>
    public static bool IsValidStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return false;

        return ValidStatuses.Contains(status);
    }

    /// <summary>
    /// Validates a status transition from currentStatus to newStatus.
    /// </summary>
    /// <param name="currentStatus">The current status of the shipment.</param>
    /// <param name="newStatus">The desired new status.</param>
    /// <returns>An Error if the transition is invalid; null if valid.</returns>
    public static Error? Validate(string currentStatus, string newStatus)
    {
        if (currentStatus == newStatus)
        {
            return new Error
            {
                Code = "INVALID_STATUS_TRANSITION",
                Message = $"Shipment is already in '{currentStatus}' status. No transition needed.",
                CorrespondingStatusCode = 409
            };
        }

        if (FinalStatuses.Contains(currentStatus))
        {
            return new Error
            {
                Code = "INVALID_STATUS_TRANSITION",
                Message = $"Shipment in '{currentStatus}' status cannot transition to any other status. '{currentStatus}' is a final state.",
                CorrespondingStatusCode = 409
            };
        }

        if (!AllowedTransitions.TryGetValue(currentStatus, out var allowed) || !allowed.Contains(newStatus))
        {
            return new Error
            {
                Code = "INVALID_STATUS_TRANSITION",
                Message = $"Invalid status transition from '{currentStatus}' to '{newStatus}'.",
                CorrespondingStatusCode = 409
            };
        }

        return null;
    }
}
