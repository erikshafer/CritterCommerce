using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseInventories.Initializing;

public sealed record InitializeItemInventory(string Sku, string FacilityId);

public sealed class InitializeItemInventoryValidator : AbstractValidator<InitializeItemInventory>
{
    public InitializeItemInventoryValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
    }
}

public static class InitializeItemInventoryEndpoint
{
    [WolverineBefore]
    public static ProblemDetails CheckOnSkuUsage(InitializeItemInventory command)
    {
        // Perhaps some business logic like querying a service to
        // check if the incoming SKU is active, etc. :)

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.WarehouseInventories)]
    [WolverinePost("/api/inventory")]
    public static (IResult, IStartStream) Post(InitializeItemInventory command)
    {
        var streamId = Guid.CreateVersion7();
        var initialized = new ItemInventoryInitialized(streamId, command.Sku, command.FacilityId);
        var start = MartenOps.StartStream<ItemInventory>(initialized);

        var location = $"/api/inventory/{start.StreamId}";
        return (Results.Created(location, start.StreamId), start);
    }
}
