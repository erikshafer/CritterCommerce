namespace Inventory.Api.WarehouseLevels;

public sealed record InventoryInitialized(Guid Id, string Sku, string FacilityId);
public sealed record InventoryIncremented(int Quantity);
public sealed record InventoryDecremented(int Quantity);

public sealed record InventoryLevel(Guid Id, string Sku, string FacilityId, int Quantity)
{
    public static InventoryLevel Create(InventoryInitialized initialized) =>
        new (initialized.Id, initialized.Sku, initialized.FacilityId, 0);

    public static InventoryLevel Apply(InventoryLevel current, InventoryIncremented incremented) =>
        current with { Quantity = current.Quantity + incremented.Quantity };

    public static InventoryLevel Apply(InventoryLevel current, InventoryDecremented decremented) =>
        current with { Quantity = current.Quantity - decremented.Quantity };

}
