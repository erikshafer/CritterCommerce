using FluentValidation;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseLevels.Initializing;

public sealed record InitializeInventory(string Sku, string FacilityId);

public sealed class InitializeInventoryValidator : AbstractValidator<InitializeInventory>
{
    public InitializeInventoryValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
    }
}

public static class InitializeInventoryHandler
{
    [WolverineBefore]
    public static ProblemDetails CheckOnSkuUsage(InitializeInventory command, IQuerySession session)
    {
        // Perhaps some business logic like querying a service to
        // check if the incoming SKU is active, etc. :)

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.WarehouseInventories)]
    [WolverinePost("/api/inventory-level")]
    public static (IResult, IStartStream) Post(InitializeInventory command)
    {
        var (sku, facilityId) = command;

        var id = CombGuidIdGeneration.NewGuid();
        var initialized = new InventoryInitialized(id, sku, facilityId);

        var start = MartenOps.StartStream<InventoryLevel>(id, initialized);

        var location = $"/api/inventory-level/{start.StreamId}";
        return (Results.Created(location, start.StreamId), start);
    }
}
