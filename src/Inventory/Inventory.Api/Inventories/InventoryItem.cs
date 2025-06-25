using JasperFx.Events;

namespace Inventory.Api.Inventories;

public record InventoryInitialized(Guid Id, string Sku);
public record InventoryIncremented(int Quantity);
public record InventoryDecremented(int Quantity);

public record InventoryItem
{
    public static InventoryItem Create(IEvent<InventoryInitialized> initialized) =>
        new() { Id = initialized.StreamId, Sku = initialized.Data.Sku };

    public Guid Id { get; set; }
    public int Version { get; set; }
    public string Sku { get; set; } = null!;
    public int Quantity { get; set; }

    public void Apply(InventoryIncremented incremented) =>
        Quantity += incremented.Quantity;

    public void Apply(InventoryDecremented decremented) =>
        Quantity -= decremented.Quantity;
}
