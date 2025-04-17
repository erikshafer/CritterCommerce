using JasperFx.Core;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Persistence;

namespace Catalog.Api.Items;

public sealed record DraftItem(string Name);

public sealed record ItemDrafted(Guid Id);

public static class DraftItemCommandHandler
{
    [Transactional]
    public static ItemDrafted Handle(DraftItem command, CatalogDbContext db)
    {
        var item = new Item { Name = command.Name };

        db.Items.Add(item);

        return new ItemDrafted(item.Id);
    }
}

public static class DraftItemEndpoints
{
    [Transactional]
    [WolverinePost("/api/items"), AlwaysPublishResponse]
    public static (ItemDrafted, Insert<Item>) Post(DraftItem command)
    {
        var item = new Item
        {
            Name = command.Name,
            Id = CombGuidIdGeneration.NewGuid(),
            Description = string.Empty
        };

        return (new ItemDrafted(item.Id), Storage.Insert(item));
    }
}

public class ItemDraftedEventHandler
{
    public void Handle(ItemDrafted @event)
    {
        Console.WriteLine("You created a new item with id {0}", @event.Id);
    }
}
