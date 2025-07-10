using FluentValidation;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseLevels.Adjusting;

public sealed record AdjustInventoryLevel(Guid InventoryLevelId, int Quantity);

public sealed class AdjustInventoryLevelValidator : AbstractValidator<AdjustInventoryLevel>
{
    public AdjustInventoryLevelValidator()
    {
        RuleFor(x => x.Quantity).NotEmpty();
    }
}

public static class AdjustInventoryLevelHandler
{
    [Tags(Tags.WarehouseInventories)]
    [WolverinePost("/api/inventory-level/{inventoryLevelId}/adjust")]
    [AggregateHandler]
    public static (IResult, Events) Handle(AdjustInventoryLevel command, InventoryLevel inventoryLevel)
    {
        var (_, quantity) = command;

        Events events = [];

        if (quantity == 0)
            throw new InvalidOperationException("Zero (0) is invalid for inventory adjustment");
        if (quantity > 0)
            events.Add(new InventoryIncremented(quantity));
        if (quantity < 0)
            events.Add(new InventoryDecremented(quantity));

        return (Results.Ok(), events);
    }
}
