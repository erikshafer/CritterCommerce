using FluentValidation;
using Wolverine;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseLevels.Adjusting;

public sealed record AdjustInventoryLevel(int Quantity);

public sealed class AdjustInventoryLevelValidator : AbstractValidator<AdjustInventoryLevel>
{
    public AdjustInventoryLevelValidator()
    {
        RuleFor(x => x.Quantity).NotEmpty();
    }
}

public static class AdjustInventoryLevelHandler
{
    [Tags(Tags.WarehouseInventoryLevels)]
    [WolverinePost("/api/inventory-level/{inventoryLevelId}/adjust")]
    public static (IResult, Events, OutgoingMessages) Handle(
        AdjustInventoryLevel command,
        [Aggregate("inventoryLevelId")] InventoryLevel inventoryLevel)
    {
        var quantity = command.Quantity;

        Events events =
        [
            quantity switch
            {
                0 => throw new InvalidOperationException("Zero (0) is invalid for inventory adjustment"),
                > 0 => new InventoryIncremented(quantity),
                < 0 => new InventoryDecremented(quantity)
            }
        ];
        OutgoingMessages messages =
        [
            new InventoryLevelAdjustmentMessage(inventoryLevel.Id, quantity)
        ];

        return (Results.Ok(), events, messages);
    }
}
