using Wolverine.Marten;

namespace Inventory.Api.WarehouseInventories;

public record AdjustInventory(Guid InventoryId, int Quantity);

public static class AdjustInventoryHandler
{
    [AggregateHandler]
    public static IEnumerable<object> Handle(AdjustInventory cmd)
    {
        var (inventoryId, quantity) = cmd;

        yield return quantity switch
        {
            0 => throw new InvalidOperationException("Why did you pass zero?"),
            < 0 => new InventoryDecremented(quantity),
            > 0 => new InventoryIncremented(quantity)
        };

        yield return new InventoryAdjustmentCompleted(inventoryId, quantity);
    }
}

/* event notification -- a possible message? -- testing purposes */

public record InventoryAdjustmentCompleted(Guid InventoryId, int Quantity);
