using FluentValidation;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseInventories.Adjusting;

public sealed record AdjustItemInventory(Guid InventoryId, int Quantity);

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
    public static (IResult, Events, OutgoingMessages) Handle(AdjustItemInventory command, ItemInventory itemInventory)
    {
        var (_, quantity) = command;

        Events events = [];
        OutgoingMessages messages = [];

        events.Add(quantity switch
        {
            0 => throw new InvalidOperationException("Zero (0) is invalid for inventory adjustment"),
            < 0 => new ItemInventoryDecremented(quantity),
            > 0 => new ItemInventoryIncremented(quantity)
        });
        messages.Add(new InventoryAdjustmentNotification(itemInventory.Id, quantity));

        return (Results.Ok(), events, messages);
    }
}
