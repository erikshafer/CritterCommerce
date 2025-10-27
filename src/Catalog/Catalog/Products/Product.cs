using JasperFx.Events;

namespace Catalog.Products;

public sealed record ProductInitialized(
    Guid Id,
    string Name,
    string Sku,
    Guid BrandId);

public sealed record Product(
    Guid Id,
    string Name,
    string Sku,
    Guid BrandId)
{
    public static Product Create(IEvent<ProductInitialized> @event) =>
        new (
            @event.StreamId,
            @event.Data.Name,
            @event.Data.Sku,
            @event.Data.BrandId
            );
}
