using JasperFx.Events;

namespace Catalog.Products;

public sealed record ProductDrafted(Guid Id, string Sku, string Name);
public sealed record DraftProductNamedChanged(string Name);
public sealed record DraftProductSkuChanged(string Sku);
public sealed record DraftProductProposed();

public sealed record DraftProduct(Guid Id, string Name, string Sku, bool HasBeenProposed = false)
{
    public static DraftProduct Create(IEvent<ProductDrafted> drafted) =>
        new (drafted.StreamId, drafted.Data.Sku, drafted.Data.Name);

    public static DraftProduct Apply(DraftProduct current, DraftProductNamedChanged nameChanged) =>
        current with { Name = nameChanged.Name };

    public static DraftProduct Apply(DraftProduct current, DraftProductSkuChanged skuChanged) =>
        current with { Sku = skuChanged.Sku };

    public static DraftProduct Apply(DraftProduct current, DraftProductProposed proposed) =>
        current with { HasBeenProposed = true };
}
