using FluentValidation;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseInventories.Adjusting;

public sealed record AdjustInventory(Guid InventoryId, int Quantity);

public sealed class AdjustInventoryValidator : AbstractValidator<AdjustInventory>
{
    public AdjustInventoryValidator()
    {
        RuleFor(x => x.Quantity).NotEmpty();
        RuleFor(x => x.Quantity).NotEqual(0);
    }
}

public static class AdjustInventoryHandler
{
    [Tags(Tags.WarehouseInventories)]
    [WolverinePost("/api/warehouse-inventories/{id}/adjust")]
    [AggregateHandler]
    public static (IResult, Events, OutgoingMessages) Handle(AdjustInventory command, InventoryItem inventory)
    {
        var (_, quantity) = command;

        Events events = [];
        OutgoingMessages messages = [];

        events.Add(quantity switch
        {
            0 => throw new InvalidOperationException("Zero (0) is invalid for inventory adjustment"),
            < 0 => new InventoryDecremented(quantity),
            > 0 => new InventoryIncremented(quantity)
        });
        messages.Add(new InventoryAdjustmentNotification(inventory.Id, quantity));

        return (Results.Ok(), events, messages);
    }
}
