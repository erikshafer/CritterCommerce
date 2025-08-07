using Legacy.Catalog.Application;
using Legacy.Catalog.Domain.Entities;
using Wolverine.Http;

namespace Legacy.Catalog.Api.Items;

public sealed record DraftItemRequest(string Name, int Id);

public sealed record ItemDraftedEvent(int Id);

public sealed record ItemDraftedResponse(int Id)
    : CreationResponse("/api/items/" + Id);

public static class DraftItemEndpoint
{
    [WolverinePost("/api/items")]
    public static (ItemDraftedResponse, ItemDraftedEvent) Handle(
        DraftItemRequest command,
        CatalogDbContext db)
    {
        var item = new Item
        {
            Name = command.Name,
            Id = command.Id,
            Description = string.Empty
        };

        db.Items.Add(item);
        db.SaveChanges();

        return (
            new ItemDraftedResponse(item.Id),
            new ItemDraftedEvent(item.Id)
        );
    }
}

public class ItemDraftedHandler
{
    public void Handle(ItemDraftedEvent @event)
    {
        Console.WriteLine("You created a new item with id {0}", @event.Id);
    }
}
