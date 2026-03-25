using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipments.Application.ExternalServices;
using Shipments.Application.Repositories;
using Shipments.Application.Results;
using Shipments.Application.Validators;
using Shipments.Domain.Models;
using Shipments.Domain.Results;

namespace Shipments.Application.UseCases.CreateShipment;

/// <summary>
/// Implementation of the CreateShipment use case.
/// </summary>
public class CreateShipmentUseCase : ICreateShipmentUseCase
{
    private readonly IShipmentRepository _repository;
    private readonly IShippingCostServiceClient _costServiceClient;
    private readonly ILogger<CreateShipmentUseCase> _logger;

    public CreateShipmentUseCase(
        IShipmentRepository repository,
        IShippingCostServiceClient costServiceClient,
        ILogger<CreateShipmentUseCase> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _costServiceClient = costServiceClient ?? throw new ArgumentNullException(nameof(costServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

        // Always call Costs API to calculate shipping cost
        var shippingCost = await _costServiceClient.CalculateShippingCostAsync(
            input.OriginZipCode!,
            input.DestinationZipCode!,
            input.Weight,
            input.Dimensions!,
            cancellationToken);

        if (shippingCost == null)
        {
            _logger.LogError("Costs API unavailable — cannot create shipment.");
            return Result<CreateShipmentOutput>.Failure(new Error
            {
                Code = "COSTS_API_UNAVAILABLE",
                Message = "The shipping cost service is temporarily unavailable. Please try again later.",
                CorrespondingStatusCode = 503
            });
        }

        // Create the shipment entity
        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            PackageName = input.PackageName!,
            Weight = input.Weight,
            Dimensions = input.Dimensions,
            ShippingCost = shippingCost.Value,
            OriginZipCode = input.OriginZipCode,
            DestinationZipCode = input.DestinationZipCode,
            DestinationAddress = input.DestinationAddress!,
            DateCreated = DateTime.UtcNow,
            DateLastUpdated = DateTime.UtcNow,
            Creator = input.Creator!,
            Status = "pending"
        };

        var createdShipment = await _repository.CreateAsync(shipment);

        _logger.LogInformation("Shipment created successfully with ID {ShipmentId}", createdShipment.Id);

        var output = new CreateShipmentOutput
        {
            Id = createdShipment.Id,
            PackageName = createdShipment.PackageName,
            Weight = createdShipment.Weight,
            Dimensions = createdShipment.Dimensions,
            ShippingCost = createdShipment.ShippingCost,
            OriginZipCode = createdShipment.OriginZipCode,
            DestinationZipCode = createdShipment.DestinationZipCode,
            DestinationAddress = createdShipment.DestinationAddress,
            DateCreated = createdShipment.DateCreated,
            DateLastUpdated = createdShipment.DateLastUpdated,
            Creator = createdShipment.Creator,
            Status = createdShipment.Status
        };

        return Result<CreateShipmentOutput>.Success(output);
    }
}

