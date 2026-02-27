using System;
using System.Collections.Generic;
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
        var validationErrors = new List<string>();

        // Validate ShipmentId is not empty
        if (string.IsNullOrWhiteSpace(input.ShipmentId))
        {
            validationErrors.Add("ShipmentId is required and cannot be empty.");
        }
        // Validate ShipmentId is a valid UUID format
        else if (!Guid.TryParse(input.ShipmentId, out _))
        {
            validationErrors.Add("ShipmentId must be a valid UUID.");
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

        return null; // Valid
    }
}
