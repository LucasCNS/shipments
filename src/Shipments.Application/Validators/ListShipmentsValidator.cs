using System;
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
        // Validate Creator - not empty
        if (string.IsNullOrWhiteSpace(input.Creator))
        {
            return new Error
            {
                Code = "EMPTY_CREATOR",
                Message = "The 'Creator' header is required.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate Status - must be a valid status or null
        if (!string.IsNullOrWhiteSpace(input.Status))
        {
            if (!IsValidStatus(input.Status))
            {
                return new Error
                {
                    Code = "INVALID_STATUS",
                    Message = $"Status must be one of: {string.Join(", ", ValidStatuses)}.",
                    CorrespondingStatusCode = 400
                };
            }
        }

        // Validate Offset - must be >= 0
        if (input.Offset < 0)
        {
            return new Error
            {
                Code = "INVALID_OFFSET",
                Message = "Offset must be greater than or equal to 0.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate Limit - must be > 0 and <= MaxLimit
        if (input.Limit <= 0)
        {
            return new Error
            {
                Code = "INVALID_LIMIT",
                Message = "Limit must be greater than 0.",
                CorrespondingStatusCode = 400
            };
        }

        if (input.Limit > MaxLimit)
        {
            return new Error
            {
                Code = "LIMIT_EXCEEDED",
                Message = $"Limit must not exceed {MaxLimit}.",
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
