using System;
using System.Collections.Generic;
using System.Linq;
using Shipments.Application.UseCases.ListShipments;
using Shipments.Domain.Results;

namespace Shipments.Application.Validators;

/// <summary>
/// Validator for ListShipments use case input.
/// </summary>
public class ListShipmentsValidator
{
    private static readonly string[] ValidStatuses = { "pending", "in_transit", "delivered", "cancelled" };
    private const int MaxLimit = 100;

    /// <summary>
    /// Validates the list shipments input.
    /// </summary>
    /// <param name="input">The input to validate.</param>
    /// <returns>Error if validation fails; null if valid.</returns>
    public static Error? Validate(ListShipmentsInput input)
    {
        var validationErrors = new List<string>();

        // Validate Creator - not empty
        if (string.IsNullOrWhiteSpace(input.Creator))
        {
            validationErrors.Add("The 'Creator' header is required.");
        }

        // Validate Status - must be a valid status or null
        if (!string.IsNullOrWhiteSpace(input.Status))
        {
            if (!IsValidStatus(input.Status))
            {
                validationErrors.Add($"Status must be one of: {string.Join(", ", ValidStatuses)}.");
            }
        }

        // Validate Offset - must be >= 0
        if (input.Offset < 0)
        {
            validationErrors.Add("Offset must be greater than or equal to 0.");
        }

        // Validate Limit - must be > 0 and <= MaxLimit
        if (input.Limit <= 0)
        {
            validationErrors.Add("Limit must be greater than 0.");
        }

        if (input.Limit > MaxLimit)
        {
            validationErrors.Add($"Limit must not exceed {MaxLimit}.");
        }

        // Return error if there are any validation errors
        if (validationErrors.Count > 0)
        {
            return new Error
            {
                Code = "VALIDATION_ERROR",
                Message = "One or more validation errors occurred.",
                ValidationErrors = validationErrors,
                CorrespondingStatusCode = 400
            };
        }

        return null;
    }

    /// <summary>
    /// Checks if the provided status is valid.
    /// </summary>
    /// <param name="status">The status to validate.</param>
    /// <returns>True if valid; false otherwise.</returns>
    private static bool IsValidStatus(string status)
    {
        return ValidStatuses.Contains(status.ToLowerInvariant());
    }
}

