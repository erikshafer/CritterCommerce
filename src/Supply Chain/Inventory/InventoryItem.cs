using JasperFx.Events;
using JasperFx.Events.Projections;
using Wolverine;
using Wolverine.Marten;

namespace Inventory;

public record InventoryInitialized(Guid Id, string Sku);

public record InventoryIncremented(int Quantity);

public record InventoryDecremented(int Quantity);

/// <summary>
/// Just a plain ole "aggregate". It has nothing to inherit or interfaced with from Marten.
/// This could be a really rich domain model with several, well guarded and potentially
/// complex operations that may not fit well in a simple + specific "read model" projection.
/// But again, you're not leveraging all the types of projection mechanisms that Marten offers
/// by having this be a bit more "free form" and primarily leveraged for live aggregations
/// when needed. However, that may suit your needs just fine!
/// </summary>
public record InventoryItem
{
    public static InventoryItem Create(IEvent<InventoryInitialized> initialized) =>
        new InventoryItem()
        {
            Id = initialized.StreamId,
            Sku = initialized.Data.Sku
        };

    public Guid Id { get; set; }
    public int Version { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }

    public void Apply(InventoryIncremented incremented)
    {
        Quantity += incremented.Quantity;
    }

    public void Apply(InventoryDecremented decremented)
    {
        Quantity -= decremented.Quantity;
    }
}
