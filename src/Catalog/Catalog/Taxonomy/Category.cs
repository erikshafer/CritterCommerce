using JasperFx.Events;

namespace Catalog.Taxonomy;

public sealed record CategoryInitialized(Guid Id, string Name, string Shorthand, string Description, Guid ParentId);

public sealed record Category(
    Guid Id,
    string Name,
    string Code,
    string Description,
    Guid ParentId)
{
    public static Category Create(IEvent<CategoryInitialized> @event) =>
        new (
            @event.StreamId,
            @event.Data.Name,
            @event.Data.Shorthand,
            @event.Data.Description,
            @event.Data.ParentId
        );
}
