using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipments.Application.Repositories;
using Shipments.Application.Results;
using Shipments.Application.StateTransitions;
using Shipments.Application.Validators;
using Shipments.Domain.Results;

namespace Shipments.Application.UseCases.UpdateShipmentStatus;

/// <summary>
/// Implementation of the UpdateShipmentStatus use case.
/// </summary>
public class UpdateShipmentStatusUseCase : IUpdateShipmentStatusUseCase
{
    private readonly IShipmentRepository _repository;
    private readonly ILogger<UpdateShipmentStatusUseCase> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateShipmentStatusUseCase class.
    /// </summary>
    /// <param name="repository">The shipment repository.</param>
    /// <param name="logger">The logger.</param>
    public UpdateShipmentStatusUseCase(IShipmentRepository repository, ILogger<UpdateShipmentStatusUseCase> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the status update of an existing shipment.
    /// </summary>
    /// <param name="input">The input data containing shipment ID and new status.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A result containing the updated shipment or error information.</returns>
    public async Task<Result<UpdateShipmentStatusOutput>> HandleAsync(UpdateShipmentStatusInput input, CancellationToken cancellationToken)
    {
        // Validate input ID is a valid UUID
        if (!Guid.TryParse(input.Id, out var shipmentId))
        {
            var error = new Error
            {
                Code = "VALIDATION_ERROR",
                Message = $"Invalid shipment ID format. Expected a valid UUID.",
                CorrespondingStatusCode = 400,
                ValidationErrors = new() { "Shipment ID must be a valid UUID." }
            };
            _logger.LogWarning("Invalid UUID format for shipment ID: {Id}", input.Id);
            return Result<UpdateShipmentStatusOutput>.Failure(error);
        }

        // Validate new status format
        var statusValidationError = UpdateShipmentStatusValidator.Validate(input.Status);
        if (statusValidationError != null)
        {
            _logger.LogWarning("Validation error for status: {Code} - {Message}", 
                statusValidationError.Code, statusValidationError.Message);
            return Result<UpdateShipmentStatusOutput>.Failure(statusValidationError);
        }

        // Fetch shipment from repository
        var shipment = await _repository.GetByIdAsync(shipmentId);
        if (shipment == null)
        {
            var error = new Error
            {
                Code = "SHIPMENT_NOT_FOUND",
                Message = $"Shipment with ID '{shipmentId}' not found.",
                CorrespondingStatusCode = 404
            };
            _logger.LogWarning("Shipment not found with ID: {ShipmentId}", shipmentId);
            return Result<UpdateShipmentStatusOutput>.Failure(error);
        }

        _logger.LogInformation("Status change request for shipment {ShipmentId}, from {CurrentStatus} to {NewStatus}", 
            shipmentId, shipment.Status, input.Status);

        // Validate state transition
        var transitionError = ShipmentStateTransitionValidator.Validate(shipment.Status ?? "pending", input.Status!);
        if (transitionError != null)
        {
            _logger.LogWarning("Invalid state transition for shipment {ShipmentId}: {Code} - {Message}", 
                shipmentId, transitionError.Code, transitionError.Message);
            return Result<UpdateShipmentStatusOutput>.Failure(transitionError);
        }

        // Update status and timestamp
        shipment.Status = input.Status;
        shipment.DateLastUpdated = DateTime.UtcNow;

        // Save updated shipment
        try
        {
            await _repository.UpdateAsync(shipment);
            _logger.LogInformation("Status updated successfully for shipment {ShipmentId}", shipmentId);
        }
        catch (Exception ex)
        {
            var error = new Error
            {
                Code = "REPOSITORY_ERROR",
                Message = "An error occurred while updating the shipment.",
                CorrespondingStatusCode = 500
            };
            _logger.LogError(ex, "Repository error updating shipment {ShipmentId}", shipmentId);
            return Result<UpdateShipmentStatusOutput>.Failure(error);
        }

        // Map shipment to output
        var output = new UpdateShipmentStatusOutput
        {
            Id = shipment.Id,
            PackageName = shipment.PackageName,
            Weight = shipment.Weight,
            Dimensions = shipment.Dimensions,
            ShippingCost = shipment.ShippingCost,
            DestinationAddress = shipment.DestinationAddress,
            DateCreated = shipment.DateCreated,
            DateLastUpdated = shipment.DateLastUpdated,
            Creator = shipment.Creator,
            Status = shipment.Status
        };

        return Result<UpdateShipmentStatusOutput>.Success(output);
    }
}
