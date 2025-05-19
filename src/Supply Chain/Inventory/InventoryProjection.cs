using Marten.Events.Aggregation;

namespace Inventory;

public class InventoryProjection : SingleStreamProjection<Inventory, Guid>
{
    public InventoryProjection()
    {

    }

    public void Apply(InventoryValidatedForUse validated, Inventory inventory)
    {
        inventory.Status = InventoryStatus.Validated;
    }

    public void Apply(InventoryIncremented incremented, Inventory inventory)
    {
        inventory.Quantity = new Quantity(inventory.Quantity + incremented.Quantity);
    }

    public void Apply(InventoryDecremented decremented, Inventory inventory)
    {
        inventory.Quantity = new Quantity(inventory.Quantity - decremented.Quantity);
    }

    public void Apply(PhysicalInventoryCountCorrection correction, Inventory inventory)
    {
        inventory.Quantity = new(correction.Quantity);
    }
}
