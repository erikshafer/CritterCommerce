using JasperFx.Events;
using Marten.Schema;

namespace Inventory.Api.WarehouseInventories;

public sealed record ItemInventoryInitialized(Guid Id, string Sku, string FacilityId);

public sealed record ItemInventoryIncremented(int Quantity);
public sealed record ItemInventoryDecremented(int Quantity);

public sealed record ItemInventory(Guid Id, string Sku, string FacilityId, int Quantity)
{
    public ItemInventory() : this(Guid.Empty, string.Empty, string.Empty, 0)
    {
    }

    public static ItemInventory Create(IEvent<ItemInventoryInitialized> initialized) =>
        new (initialized.StreamId, initialized.Data.Sku, initialized.Data.FacilityId, 0);

    public static ItemInventory Apply(ItemInventory current, ItemInventoryIncremented incremented) =>
        current with { Quantity = current.Quantity + incremented.Quantity };

    public static ItemInventory Apply(ItemInventory current, ItemInventoryDecremented decremented) =>
        current with { Quantity = current.Quantity - decremented.Quantity };

}
