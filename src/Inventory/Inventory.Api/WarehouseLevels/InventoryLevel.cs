using JasperFx.Events;

namespace Inventory.Api.WarehouseLevels;

public sealed record InventoryInitialized(string Sku, string FacilityId, string FacilityLotId);
public sealed record InventoryIncremented(int Quantity);
public sealed record InventoryDecremented(int Quantity);
public sealed record InventoryLotMoved(string FacilityLotId);

public sealed record InventoryLevel(Guid Id, string Sku, string FacilityId, string FacilityLotId, int Quantity)
{
    public static InventoryLevel Create(IEvent<InventoryInitialized> initialized) =>
        new (initialized.StreamId, initialized.Data.Sku, initialized.Data.FacilityId, initialized.Data.FacilityLotId, 0);

    public static InventoryLevel Apply(InventoryLevel current, InventoryIncremented incremented) =>
        current with { Quantity = current.Quantity + incremented.Quantity };

    public static InventoryLevel Apply(InventoryLevel current, InventoryDecremented decremented) =>
        current with { Quantity = current.Quantity + decremented.Quantity };

    public static InventoryLevel Apply(InventoryLevel current, InventoryLotMoved moved) =>
        current with { FacilityLotId = moved.FacilityLotId };
}
