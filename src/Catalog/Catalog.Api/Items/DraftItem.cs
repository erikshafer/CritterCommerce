using JasperFx.Core;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Persistence;

namespace Catalog.Api.Items;

public sealed record DraftItem(string Name);

public sealed record ItemDrafted(Guid Id);

public sealed record ItemDraftedResponse(Guid Id)
    : CreationResponse("/api/items/" + Id);

public static class DraftItemEndpoint
{
    [WolverinePost("/api/items")]
    public static (ItemDraftedResponse, ItemDrafted) Handle(
        DraftItem command,
        CatalogDbContext db)
    {
        var item = new Item
        {
            Name = command.Name,
            Id = CombGuidIdGeneration.NewGuid(),
            Description = string.Empty
        };

        db.Items.Add(item);

        return (
            new ItemDraftedResponse(item.Id),
            new ItemDrafted(item.Id)
        );
    }
}

public class ItemDraftedHandler
{
    public void Handle(ItemDrafted @event)
    {
        Console.WriteLine("You created a new item with id {0}", @event.Id);
    }
}
