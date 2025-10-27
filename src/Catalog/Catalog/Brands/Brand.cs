using JasperFx.Events;

namespace Catalog.Brands;

public sealed record BrandInitialized(
    Guid Id,
    string Name);

public sealed record Brand(
    Guid Id,
    string Name)
{
    public static Brand Create(IEvent<BrandInitialized> @event) =>
        new(@event.StreamId, @event.Data.Name);
}
