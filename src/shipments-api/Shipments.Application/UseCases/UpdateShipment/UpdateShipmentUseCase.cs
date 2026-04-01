using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipments.Application.ExternalServices;
using Shipments.Application.Repositories;
using Shipments.Application.Results;
using Shipments.Application.Validators;
using Shipments.Domain.Models;

namespace Shipments.Application.UseCases.UpdateShipment;

/// <summary>
/// Implementation of the UpdateShipment use case.
/// </summary>
public class UpdateShipmentUseCase : IUpdateShipmentUseCase
{
    private readonly IShipmentRepository _repository;
    private readonly IShippingCostServiceClient _costServiceClient;
    private readonly ILogger<UpdateShipmentUseCase> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateShipmentUseCase class.
    /// </summary>
    /// <param name="repository">The shipment repository.</param>
    /// <param name="costServiceClient">The shipping cost service client.</param>
    /// <param name="logger">The logger.</param>
    public UpdateShipmentUseCase(IShipmentRepository repository, IShippingCostServiceClient costServiceClient, ILogger<UpdateShipmentUseCase> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _costServiceClient = costServiceClient ?? throw new ArgumentNullException(nameof(costServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the update of an existing shipment.
    /// </summary>
    /// <param name="input">The input data for updating a shipment.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The shipment update output.</returns>
    public async Task<Result<UpdateShipmentOutput>> HandleAsync(UpdateShipmentInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request to update shipment with ID: {ShipmentId}", input.Id);

        // Validate input
        var validationError = UpdateShipmentValidator.Validate(input);
        if (validationError != null)
        {
            _logger.LogWarning("Validation error when updating shipment: {Code} - {Message}",
                validationError.Code, validationError.Message);

            return Result<UpdateShipmentOutput>.Failure(validationError);
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

            return Result<UpdateShipmentOutput>.Failure(error);
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

            return Result<UpdateShipmentOutput>.Failure(error);
        }

        // Check if data fields are being updated
        var hasDataFields = !string.IsNullOrWhiteSpace(input.PackageName) ||
                           input.Weight.HasValue ||
                           input.Dimensions != null ||
                           !string.IsNullOrWhiteSpace(input.DestinationAddress);

        // Data fields can only be updated when status is "pending"
        if (hasDataFields && existingShipment.Status != "pending")
        {
            _logger.LogWarning("Attempt to update data fields of shipment in non-pending status: {ShipmentId}, Status: {Status}",
                shipmentId, existingShipment.Status);

            var error = new Domain.Results.Error
            {
                Code = "SHIPMENT_NOT_UPDATABLE",
                Message = $"Shipment with status '{existingShipment.Status}' cannot have its data fields updated. Only shipments with 'pending' status can have data fields updated.",
                CorrespondingStatusCode = 409
            };

            return Result<UpdateShipmentOutput>.Failure(error);
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

        if (!string.IsNullOrWhiteSpace(input.DestinationAddress))
        {
            existingShipment.DestinationAddress = input.DestinationAddress;
        }

        // Recalculate shipping cost when weight or dimensions changed
        if (input.Weight.HasValue || input.Dimensions != null)
        {
            if (string.IsNullOrWhiteSpace(existingShipment.OriginZipCode) ||
                string.IsNullOrWhiteSpace(existingShipment.DestinationZipCode))
            {
                var missingZipError = new Domain.Results.Error
                {
                    Code = "MISSING_ZIP_CODES",
                    Message = "Cannot recalculate shipping cost: shipment has no ZIP codes stored.",
                    CorrespondingStatusCode = 409
                };
                return Result<UpdateShipmentOutput>.Failure(missingZipError);
            }

            _logger.LogInformation("Recalculating shipping cost for shipment {ShipmentId}", shipmentId);
            var newCost = await _costServiceClient.CalculateShippingCostAsync(
                existingShipment.OriginZipCode,
                existingShipment.DestinationZipCode,
                existingShipment.Weight,
                existingShipment.Dimensions!,
                cancellationToken);

            if (newCost == null)
            {
                _logger.LogWarning("Costs API unavailable while recalculating for shipment {ShipmentId}", shipmentId);
                return Result<UpdateShipmentOutput>.Failure(new Domain.Results.Error
                {
                    Code = "COSTS_API_UNAVAILABLE",
                    Message = "Unable to recalculate shipping cost: Costs API is currently unavailable.",
                    CorrespondingStatusCode = 503
                });
            }

            existingShipment.ShippingCost = newCost.Value;
        }

        // Update the last modified date
        existingShipment.DateLastUpdated = DateTime.UtcNow;

        // Save to repository
        var updatedShipment = await _repository.UpdateAsync(existingShipment);

        _logger.LogInformation("Shipment updated successfully with ID {ShipmentId}", shipmentId);

        // Return output
        return Result<UpdateShipmentOutput>.Success(new UpdateShipmentOutput
        {
            Id = updatedShipment!.Id,
            PackageName = updatedShipment.PackageName,
            Weight = updatedShipment.Weight,
            Dimensions = updatedShipment.Dimensions,
            ShippingCost = updatedShipment.ShippingCost,
            OriginZipCode = updatedShipment.OriginZipCode,
            DestinationZipCode = updatedShipment.DestinationZipCode,
            DestinationAddress = updatedShipment.DestinationAddress,
            DateCreated = updatedShipment.DateCreated,
            DateLastUpdated = updatedShipment.DateLastUpdated,
            Creator = updatedShipment.Creator,
            Status = updatedShipment.Status
        });
    }
}
