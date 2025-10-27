using JasperFx.Events;

namespace Catalog.DraftProducts;

public sealed record ProductDrafted(Guid Id, string Name, string? InternalNotes, string? Sku, Guid? BrandId);
public sealed record DraftProductNamedChanged(string Name);
public sealed record DraftProductInternalNotesChanged(string InternalNotes);
public sealed record DraftProductSkuChanged(string Sku);
public sealed record DraftProductBrandChanged(Guid BrandId);
public sealed record DraftProductSkuValidated(string Sku);
public sealed record DraftProductBrandValidated(Guid BrandId);
public sealed record DraftProductApproved();

public sealed record DraftProduct(
    Guid Id,
    string Name,
    string? InternalNotes = null,
    string? Sku = null,
    Guid? BrandId = null,
    bool SkuHasBeenValidated = false,
    bool BrandHasBeenValidated = false,
    bool HasBeenApproved = false)
{
    public static DraftProduct Create(IEvent<ProductDrafted> @event) =>
        new (
            @event.StreamId,
            @event.Data.Name,
            @event.Data.InternalNotes,
            @event.Data.Sku,
            @event.Data.BrandId
            );

    public static DraftProduct Apply(DraftProduct current, DraftProductNamedChanged @event) =>
        current with { Name = @event.Name };

    public static DraftProduct Apply(DraftProduct current, DraftProductInternalNotesChanged @event) =>
        current with { Name = @event.InternalNotes };

    public static DraftProduct Apply(DraftProduct current, DraftProductSkuChanged @event) =>
        current with { Sku = @event.Sku };

    public static DraftProduct Apply(DraftProduct current, DraftProductBrandChanged @event) =>
        current with { BrandId = @event.BrandId };

    public static DraftProduct Apply(DraftProduct current, DraftProductSkuValidated @event) =>
        current with { SkuHasBeenValidated = true };

    public static DraftProduct Apply(DraftProduct current, DraftProductBrandValidated @event) =>
        current with { BrandHasBeenValidated = true };

    public static DraftProduct Apply(DraftProduct current, DraftProductApproved @event) =>
        current with { HasBeenApproved = true };
}
