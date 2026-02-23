using System;
using Shipments.Application.UseCases.GetShipmentById;
using Shipments.Domain.Results;

namespace Shipments.Application.Validators;

/// <summary>
/// Validator for GetShipmentById use case input.
/// </summary>
public class GetShipmentByIdValidator
{
    /// <summary>
    /// Validates the get shipment by ID input.
    /// </summary>
    /// <param name="input">The input to validate.</param>
    /// <returns>Error if validation fails; null if valid.</returns>
    public static Error? Validate(GetShipmentByIdInput input)
    {
        // Validate ShipmentId is not empty
        if (string.IsNullOrWhiteSpace(input.ShipmentId))
        {
            return new Error
            {
                Code = "EMPTY_SHIPMENT_ID",
                Message = "ShipmentId is required and cannot be empty.",
                CorrespondingStatusCode = 400
            };
        }

        // Validate ShipmentId is a valid UUID format
        if (!Guid.TryParse(input.ShipmentId, out _))
        {
            return new Error
            {
                Code = "INVALID_UUID_FORMAT",
                Message = "ShipmentId must be a valid UUID.",
                CorrespondingStatusCode = 400
            };
        }

        return null; // Valid
    }
}
