using JasperFx.Events;

namespace Inventory.Api.WarehouseInventories;

public record ItemInventoryInitialized(Guid Id, string Sku, string FacilityId);

public record ItemInventoryIncremented(int Quantity);
public record ItemInventoryDecremented(int Quantity);

public sealed record ItemInventory
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = null!;
    public string FacilityId { get; set; } = null!;
    public int Quantity { get; set; }

    public ItemInventory() : this(Guid.Empty, string.Empty, string.Empty, 0)
    {
    }

    public ItemInventory(Guid id, string sku, string facilityId, int quantity)
    {
        Id = id;
        Sku = sku;
        FacilityId = facilityId;
        Quantity = quantity;
    }

    public static ItemInventory Create(IEvent<ItemInventoryInitialized> initialized) =>
        new (
            initialized.StreamId,
            initialized.Data.Sku,
            initialized.Data.FacilityId,
            0
        );

    public void Apply(ItemInventory current, ItemInventoryIncremented incremented) =>
        Quantity += incremented.Quantity;

    public void Apply(ItemInventoryDecremented decremented) =>
        Quantity -= decremented.Quantity;
}
