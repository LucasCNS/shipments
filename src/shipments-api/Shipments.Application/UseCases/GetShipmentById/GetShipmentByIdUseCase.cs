using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipments.Application.Repositories;
using Shipments.Application.Validators;

namespace Shipments.Application.UseCases.GetShipmentById;

/// <summary>
/// Implementation of the GetShipmentById use case.
/// </summary>
public class GetShipmentByIdUseCase : IGetShipmentByIdUseCase
{
    private readonly IShipmentRepository _repository;
    private readonly ILogger<GetShipmentByIdUseCase> _logger;

    /// <summary>
    /// Initializes a new instance of the GetShipmentByIdUseCase class.
    /// </summary>
    /// <param name="repository">The shipment repository.</param>
    /// <param name="logger">The logger.</param>
    public GetShipmentByIdUseCase(IShipmentRepository repository, ILogger<GetShipmentByIdUseCase> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the retrieval of a shipment by its unique identifier.
    /// </summary>
    /// <param name="input">The input data containing the shipment ID.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The shipment retrieval output.</returns>
    public async Task<GetShipmentByIdOutput> HandleAsync(GetShipmentByIdInput input, CancellationToken cancellationToken)
    {
        // Validate input
        var validationError = GetShipmentByIdValidator.Validate(input);
        if (validationError != null)
        {
            _logger.LogWarning("Validation error when retrieving shipment: {Code} - {Message}",
                validationError.Code, validationError.Message);

            return new GetShipmentByIdOutput
            {
                Error = validationError
            };
        }

        // Parse the UUID
        if (!Guid.TryParse(input.ShipmentId, out var shipmentId))
        {
            var error = new Shipments.Domain.Results.Error
            {
                Code = "INVALID_UUID_FORMAT",
                Message = "ShipmentId must be a valid UUID.",
                CorrespondingStatusCode = 400
            };

            _logger.LogWarning("Invalid UUID format provided: {ShipmentId}", input.ShipmentId);

            return new GetShipmentByIdOutput
            {
                Error = error
            };
        }

        try
        {
            _logger.LogInformation("Retrieving shipment with ID: {ShipmentId}", shipmentId);

            // Query repository
            var shipment = await _repository.GetByIdAsync(shipmentId);

            // Check if shipment exists
            if (shipment == null)
            {
                var notFoundError = new Shipments.Domain.Results.Error
                {
                    Code = "SHIPMENT_NOT_FOUND",
                    Message = $"Shipment with ID {shipmentId} not found.",
                    CorrespondingStatusCode = 404
                };

                _logger.LogWarning("Shipment not found with ID: {ShipmentId}", shipmentId);

                return new GetShipmentByIdOutput
                {
                    Error = notFoundError
                };
            }

            _logger.LogInformation("Shipment found successfully with ID: {ShipmentId}", shipmentId);

            // Return the shipment data
            return new GetShipmentByIdOutput
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
                Status = shipment.Status,
                Error = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shipment with ID: {ShipmentId}", shipmentId);

            var errorResult = new Shipments.Domain.Results.Error
            {
                Code = "REPOSITORY_ERROR",
                Message = "An error occurred while retrieving the shipment.",
                CorrespondingStatusCode = 500
            };

            return new GetShipmentByIdOutput
            {
                Error = errorResult
            };
        }
    }
}
