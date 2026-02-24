using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipments.Application.Repositories;
using Shipments.Application.Validators;
using Shipments.Domain.Models;

namespace Shipments.Application.UseCases.UpdateShipment;

/// <summary>
/// Implementation of the UpdateShipment use case.
/// </summary>
public class UpdateShipmentUseCase : IUpdateShipmentUseCase
{
    private readonly IShipmentRepository _repository;
    private readonly ILogger<UpdateShipmentUseCase> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateShipmentUseCase class.
    /// </summary>
    /// <param name="repository">The shipment repository.</param>
    /// <param name="logger">The logger.</param>
    public UpdateShipmentUseCase(IShipmentRepository repository, ILogger<UpdateShipmentUseCase> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the update of an existing shipment.
    /// </summary>
    /// <param name="input">The input data for updating a shipment.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The shipment update output.</returns>
    public async Task<UpdateShipmentOutput> HandleAsync(UpdateShipmentInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request to update shipment with ID: {ShipmentId}", input.Id);

        // Validate input
        var validationError = UpdateShipmentValidator.Validate(input);
        if (validationError != null)
        {
            _logger.LogWarning("Validation error when updating shipment: {Code} - {Message}",
                validationError.Code, validationError.Message);

            return new UpdateShipmentOutput
            {
                Error = validationError
            };
        }

        // Parse the ID
        if (!Guid.TryParse(input.Id, out var shipmentId))
        {
            var error = new Domain.Results.Error
            {
                Code = "INVALID_SHIPMENT_ID",
                Message = "ShipmentId must be a valid GUID.",
                CorrespondingStatusCode = 400
            };

            return new UpdateShipmentOutput
            {
                Error = error
            };
        }

        // Retrieve existing shipment
        var existingShipment = await _repository.GetByIdAsync(shipmentId);
        if (existingShipment == null)
        {
            _logger.LogWarning("Attempt to update non-existent shipment with ID: {ShipmentId}", shipmentId);

            var error = new Domain.Results.Error
            {
                Code = "SHIPMENT_NOT_FOUND",
                Message = "Shipment with the specified ID does not exist.",
                CorrespondingStatusCode = 404
            };

            return new UpdateShipmentOutput
            {
                Error = error
            };
        }

        // Validate shipment status is "pending"
        if (existingShipment.Status != "pending")
        {
            _logger.LogWarning("Attempt to update shipment in non-pending status: {ShipmentId}, Status: {Status}",
                shipmentId, existingShipment.Status);

            var error = new Domain.Results.Error
            {
                Code = "SHIPMENT_NOT_UPDATABLE",
                Message = $"Shipment with status '{existingShipment.Status}' cannot be updated. Only shipments with 'pending' status can be updated.",
                CorrespondingStatusCode = 409
            };

            return new UpdateShipmentOutput
            {
                Error = error
            };
        }

        // Check if at least one field is provided for update
        var hasFieldsToUpdate = !string.IsNullOrWhiteSpace(input.PackageName) ||
                               input.Weight.HasValue ||
                               input.Dimensions != null ||
                               input.ShippingCost.HasValue ||
                               !string.IsNullOrWhiteSpace(input.DestinationAddress);

        if (!hasFieldsToUpdate)
        {
            var error = new Domain.Results.Error
            {
                Code = "NO_FIELDS_TO_UPDATE",
                Message = "At least one field must be provided for update.",
                CorrespondingStatusCode = 400
            };

            return new UpdateShipmentOutput
            {
                Error = error
            };
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(input.PackageName))
        {
            existingShipment.PackageName = input.PackageName;
        }

        if (input.Weight.HasValue)
        {
            existingShipment.Weight = input.Weight.Value;
        }

        if (input.Dimensions != null)
        {
            existingShipment.Dimensions = input.Dimensions;
        }

        if (input.ShippingCost.HasValue)
        {
            existingShipment.ShippingCost = input.ShippingCost.Value;
        }

        if (!string.IsNullOrWhiteSpace(input.DestinationAddress))
        {
            existingShipment.DestinationAddress = input.DestinationAddress;
        }

        // Update the last modified date
        existingShipment.DateLastUpdated = DateTime.UtcNow;

        // Save to repository
        var updatedShipment = await _repository.UpdateAsync(existingShipment);

        _logger.LogInformation("Shipment updated successfully with ID {ShipmentId}", shipmentId);

        // Return output
        return new UpdateShipmentOutput
        {
            Id = updatedShipment!.Id,
            PackageName = updatedShipment.PackageName,
            Weight = updatedShipment.Weight,
            Dimensions = updatedShipment.Dimensions,
            ShippingCost = updatedShipment.ShippingCost,
            DestinationAddress = updatedShipment.DestinationAddress,
            DateCreated = updatedShipment.DateCreated,
            DateLastUpdated = updatedShipment.DateLastUpdated,
            Creator = updatedShipment.Creator,
            Status = updatedShipment.Status,
            Error = null
        };
    }
}
