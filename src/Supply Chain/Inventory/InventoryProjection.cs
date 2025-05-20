using Marten.Events.Aggregation;

namespace Inventory;

public class InventoryProjection : SingleStreamProjection<Inventory, Guid>
{
    public static Inventory Create(InventoryInitialized initialized) =>
        new(initialized);

    public void Apply(InventoryValidatedForUse validated, Inventory inventory) =>
        inventory.Status = InventoryStatus.Validated;

    public void Apply(InventoryIncremented incremented, Inventory inventory) =>
        inventory.Quantity = inventory.Quantity + incremented.Quantity;

    public void Apply(InventoryDecremented decremented, Inventory inventory) =>
        inventory.Quantity = inventory.Quantity - decremented.Quantity;

    public void Apply(PhysicalInventoryCountCorrection correction, Inventory inventory) =>
        inventory.Quantity = correction.Quantity;
}
