using JasperFx.Events;

namespace Inventory.Api.WarehouseInventories;

public record InventoryInitialized(Guid Id, string Sku);
public record InventoryIncremented(int Quantity);
public record InventoryDecremented(int Quantity);

public sealed record InventoryItem
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = null!;
    public int Quantity { get; set; }

    public InventoryItem() : this(Guid.Empty, string.Empty, 0)
    {
    }

    public InventoryItem(Guid id, string sku, int quantity)
    {
        Id = id;
        Sku = sku;
        Quantity = quantity;
    }

    public static InventoryItem Create(IEvent<InventoryInitialized> initialized) =>
        new (
            initialized.StreamId,
            initialized.Data.Sku,
            0
        );

    public void Apply(InventoryItem current, InventoryIncremented incremented) =>
        Quantity += incremented.Quantity;

    public void Apply(InventoryDecremented decremented) =>
        Quantity -= decremented.Quantity;
}
