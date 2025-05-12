using Marten;

namespace Inventory;

public record InventoryInitialized(string Sku);

public record InventoryMarkedReady(string InventoryId);

public record InventoryIncremented(string InventoryId, int Quantity);

public record InventoryDecremented(string InventoryId, int Quantity);

public enum InventoryStatus
{
    Unset = 0,
    Initialized = 1,
    Ready = 2,
    InStock = 3,
    OutOfStock = 4
}

public class Inventory
{
    public Guid Id { get; set; }
    public Sku Sku { get; set; }
    public Quantity Quantity { get; set; }
    public InventoryStatus Status { get; set; }

    // Referencing Marten here as a shorthand example. I prefer decoupling
    // the two (domain model / aggregate and libraries / frameworks), but
    // you can do so!
    public static async Task<InventoryInitialized> Initialize(Sku sku, IQuerySession session)
    {
        // scaffolding

        var verifiedSku = await session.LoadAsync<SkuView>(sku.Value);

        return new InventoryInitialized(sku.Value);
    }
}

public sealed record SkuView(string Value);
