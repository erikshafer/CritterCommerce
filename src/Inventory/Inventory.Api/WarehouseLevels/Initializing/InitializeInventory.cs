using FluentValidation;
using Inventory.Api.Locations;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseLevels.Initializing;

public sealed record InitializeInventory(string Sku, string Facility, string FacilityLotId);

public sealed class InitializeInventoryValidator : AbstractValidator<InitializeInventory>
{
    public InitializeInventoryValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
        RuleFor(x => x.Facility).NotEmpty();
        RuleFor(x => x.FacilityLotId).NotEmpty();
        RuleFor(x => x.FacilityLotId).Matches(@"^[A-Z0-9\-]+$");
    }
}

public static class InitializeInventoryHandler
{
    [WolverineBefore]
    public static async Task<ProblemDetails> CheckFacilityAndLot(
        InitializeInventory command,
        IQuerySession session,
        IFacilityLotService service)
    {
        var (_, facility, facilityLotId) = command;

        var facilityLocation = await session
            .Query<Location>()
            .Where(loc => loc.Name == facility)
            .FirstOrDefaultAsync();
        if (facilityLocation is null)
            return new ProblemDetails
            {
                Detail = $"Facility '{facility}' is not available",
                Status = StatusCodes.Status412PreconditionFailed
            };

        var isLotAvailable = service.IsLotAvailable(facilityLotId);
        if (isLotAvailable is false)
            return new ProblemDetails
            {
                Detail = $"Lot '{facilityLotId}' at Facility '{facility}' is not available",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.WarehouseInventoryLevels)]
    [WolverinePost("/api/inventory-level")]
    public static (IResult, IStartStream) Post(InitializeInventory command)
    {
        var initialized = new InventoryInitialized(command.Sku, command.Facility, command.FacilityLotId);
        var start = MartenOps.StartStream<InventoryLevel>(initialized);

        var location = $"/api/inventory-level/{start.StreamId}";
        return (Results.Created(location, start.StreamId), start);
    }
}
