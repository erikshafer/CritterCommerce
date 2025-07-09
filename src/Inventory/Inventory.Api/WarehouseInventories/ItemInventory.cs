namespace Inventory.Api.WarehouseInventories;

public sealed record ItemInventoryInitialized(Guid Id, string Sku, string FacilityId);
public sealed record ItemInventoryIncremented(int Quantity);
public sealed record ItemInventoryDecremented(int Quantity);

public sealed record ItemInventory(Guid Id, string Sku, string FacilityId, int Quantity)
{
    public static ItemInventory Create(ItemInventoryInitialized initialized) =>
        new (initialized.Id, initialized.Sku, initialized.FacilityId, 0);

    public static ItemInventory Apply(ItemInventory current, ItemInventoryIncremented incremented) =>
        current with { Quantity = current.Quantity + incremented.Quantity };

    public static ItemInventory Apply(ItemInventory current, ItemInventoryDecremented decremented) =>
        current with { Quantity = current.Quantity - decremented.Quantity };

}
