using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseInventories;

public sealed record InitializeInventory(string Sku);

public sealed class InitializeInventoryValidator : AbstractValidator<InitializeInventory>
{
    public InitializeInventoryValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
    }
}

public static class InitializeInventoryEndpoint
{
    [WolverineBefore]
    public static ProblemDetails CheckOnSkuUsage(InitializeInventory command)
    {
        // Perhaps some business logic like querying a service to
        // check if the incoming SKU is active, etc. :)

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.WarehouseInventories)]
    [WolverinePost("/api/inventory")]
    public static (IResult, IStartStream) Post(InitializeInventory command)
    {
        var streamId = Guid.CreateVersion7();
        var initialized = new InventoryInitialized(streamId, command.Sku);
        var start = MartenOps.StartStream<InventoryItem>(initialized);

        var location = $"/api/inventory/{start.StreamId}";
        return (Results.Created(location, start.StreamId), start);
    }
}
