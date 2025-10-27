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
    public static Category Create(IEvent<CategoryInitialized> drafted) =>
        new (
            drafted.StreamId,
            drafted.Data.Name,
            drafted.Data.Shorthand,
            drafted.Data.Description,
            drafted.Data.ParentId
        );
}
