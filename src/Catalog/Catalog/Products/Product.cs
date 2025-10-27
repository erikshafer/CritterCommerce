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
    public static Product Create(IEvent<ProductInitialized> drafted) =>
        new (
            drafted.StreamId,
            drafted.Data.Name,
            drafted.Data.Sku,
            drafted.Data.BrandId
            );
}
