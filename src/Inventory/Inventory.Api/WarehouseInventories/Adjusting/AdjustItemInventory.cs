using FluentValidation;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseInventories.Adjusting;

public sealed record AdjustItemInventory(
    Guid ItemInventoryId,
    int Quantity
);

public sealed class AdjustItemInventoryValidator : AbstractValidator<AdjustItemInventory>
{
    public AdjustItemInventoryValidator()
    {
        RuleFor(x => x.Quantity).NotEmpty();
        RuleFor(x => x.Quantity).NotEqual(0);
    }
}

public static class AdjustItemInventoryHandler
{
    [Tags(Tags.WarehouseInventories)]
    [WolverinePost("/api/warehouse-inventories/{id}/adjust")]
    [AggregateHandler]
    public static (IResult, Events) Handle(AdjustItemInventory command, ItemInventory itemInventory)
    {
        var (_, quantity) = command;

        Events events = [];

        if (quantity == 0)
            throw new InvalidOperationException("Zero (0) is invalid for inventory adjustment");
        if (quantity > 0)
            events.Add(new ItemInventoryIncremented(quantity));
        if (quantity < 0)
            events.Add(new ItemInventoryDecremented(quantity));

        return (Results.Ok(), events);
    }
}
