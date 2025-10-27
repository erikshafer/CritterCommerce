using JasperFx.Events;

namespace Catalog.DraftTaxonomy;

public sealed record CategoryDrafted(Guid Id, string Name, string? Shorthand, string? Description, Guid? ParentId);
public sealed record DraftCategoryNameChanged(string Name);
public sealed record DraftCategoryCodeChanged(string Code);
public sealed record DraftCategoryParentChanged(Guid ParentId, string ParentName, string ParentCode);
public sealed record DraftCategoryDescriptionChanged(string Description);
public sealed record DraftCategoryApproved();

public sealed record DraftCategory(
    Guid Id,
    string Name,
    string? Code = null,
    string? Description = null,
    Guid? ParentId = null,
    string? ParentName = null,
    string? ParentCode = null,
    bool HasBeenApproved = false)
{
    public static DraftCategory Create(IEvent<CategoryDrafted> drafted) =>
        new (
            drafted.StreamId,
            drafted.Data.Name,
            drafted.Data.Shorthand,
            drafted.Data.Description,
            drafted.Data.ParentId
            );

    public static DraftCategory Apply(DraftCategory current, DraftCategoryNameChanged @event) =>
        current with { Name = @event.Name };

    public static DraftCategory Apply(DraftCategory current, DraftCategoryCodeChanged @event) =>
        current with { Name = @event.Code };

    public static DraftCategory Apply(DraftCategory current, DraftCategoryDescriptionChanged @event) =>
        current with { Name = @event.Description };

    public static DraftCategory Apply(DraftCategory current, DraftCategoryParentChanged @event) =>
        current with { ParentId = @event.ParentId, ParentName = @event.ParentName, ParentCode = @event.ParentCode };

    public static DraftCategory Apply(DraftCategory current, DraftCategoryApproved @event) =>
        current with { HasBeenApproved = true };
}
