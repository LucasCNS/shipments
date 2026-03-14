using Shipments.Domain.Results;

namespace Shipments.Application.Validators;

/// <summary>
/// Validator for UpdateShipmentStatus use case input status field.
/// </summary>
public static class UpdateShipmentStatusValidator
{
    /// <summary>
    /// Validates that a status string is one of the valid shipment statuses.
    /// </summary>
    /// <param name="newStatus">The status value to validate.</param>
    /// <returns>An Error if the status is invalid; null if valid.</returns>
    public static Error? Validate(string? newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
        {
            return new Error
            {
                Code = "VALIDATION_ERROR",
                Message = "Status is required and cannot be empty.",
                CorrespondingStatusCode = 400,
                ValidationErrors = new() { "Status is required and cannot be empty." }
            };
        }

        if (!StateTransitions.ShipmentStateTransitionValidator.IsValidStatus(newStatus))
        {
            return new Error
            {
                Code = "VALIDATION_ERROR",
                Message = $"'{newStatus}' is not a valid shipment status. Valid statuses are: pending, in_transit, delivered, cancelled.",
                CorrespondingStatusCode = 400,
                ValidationErrors = new() { $"'{newStatus}' is not a valid shipment status." }
            };
        }

        return null;
    }
}
