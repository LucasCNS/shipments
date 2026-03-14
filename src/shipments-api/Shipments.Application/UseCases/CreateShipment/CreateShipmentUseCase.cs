using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipments.Application.Repositories;
using Shipments.Application.Results;
using Shipments.Application.Validators;
using Shipments.Domain.Models;

namespace Shipments.Application.UseCases.CreateShipment;

/// <summary>
/// Implementation of the CreateShipment use case.
/// </summary>
public class CreateShipmentUseCase : ICreateShipmentUseCase
{
    private readonly IShipmentRepository _repository;
    private readonly ILogger<CreateShipmentUseCase> _logger;

    /// <summary>
    /// Initializes a new instance of the CreateShipmentUseCase class.
    /// </summary>
    /// <param name="repository">The shipment repository.</param>
    /// <param name="logger">The logger.</param>
    public CreateShipmentUseCase(IShipmentRepository repository, ILogger<CreateShipmentUseCase> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the creation of a new shipment.
    /// </summary>
    /// <param name="input">The input data for creating a shipment.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A result containing the shipment creation output or error information.</returns>
    public async Task<Result<CreateShipmentOutput>> HandleAsync(CreateShipmentInput input, CancellationToken cancellationToken)
    {
        // Validate input
        var validationError = CreateShipmentValidator.Validate(input);
        if (validationError != null)
        {
            _logger.LogWarning("Validation error when creating shipment: {Code} - {Message}", 
                validationError.Code, validationError.Message);

            return Result<CreateShipmentOutput>.Failure(validationError);
        }

        // Create the shipment entity
        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            PackageName = input.PackageName!,
            Weight = input.Weight,
            Dimensions = input.Dimensions,
            ShippingCost = input.ShippingCost,
            DestinationAddress = input.DestinationAddress!,
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = input.Creator!,
            Status = "pending"
        };

        // Save to repository
        var createdShipment = await _repository.CreateAsync(shipment);

        _logger.LogInformation("Shipment created successfully with ID {ShipmentId}", createdShipment.Id);

        // Return output
        var output = new CreateShipmentOutput
        {
            Id = createdShipment.Id,
            PackageName = createdShipment.PackageName,
            Weight = createdShipment.Weight,
            Dimensions = createdShipment.Dimensions,
            ShippingCost = createdShipment.ShippingCost,
            DestinationAddress = createdShipment.DestinationAddress,
            DateCreated = createdShipment.DateCreated,
            DateLastUpdated = createdShipment.DateLastUpdated,
            Creator = createdShipment.Creator,
            Status = createdShipment.Status
        };

        return Result<CreateShipmentOutput>.Success(output);
    }
}
