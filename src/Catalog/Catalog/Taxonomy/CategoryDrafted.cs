using JasperFx.Events;

namespace Catalog.Taxonomy;

public sealed record CategoryDrafted(Guid Id, string Name, string Shorthand);
public sealed record DraftCategoryNameChanged(string Name);
public sealed record DraftCategoryShorthandChanged(string Shorthand);
public sealed record DraftCategoryProposed();

public sealed record DraftCategory(Guid Id, string Name, string Shorthand, bool HasBeenProposed = false)
{
    public static DraftCategory Create(IEvent<CategoryDrafted> drafted) =>
        new (drafted.StreamId, drafted.Data.Name, drafted.Data.Shorthand);

    public static DraftCategory Apply(DraftCategory current, DraftCategoryNameChanged nameChanged) =>
        current with { Name = nameChanged.Name };

    public static DraftCategory Apply(DraftCategory current, DraftCategoryShorthandChanged shorthandChanged) =>
        current with { Name = shorthandChanged.Shorthand };

    public static DraftCategory Apply(DraftCategory current, DraftCategoryProposed proposed) =>
        current with { HasBeenProposed = true };
}
