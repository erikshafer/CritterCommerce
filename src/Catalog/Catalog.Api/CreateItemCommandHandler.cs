using JasperFx.Core;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Persistence;

namespace Catalog.Api;

public sealed record ItemCreated(Guid Id);

public sealed record CreateItemCommand(string Name);

public static class CreateItemCommandHandler
{
    [Transactional]
    public static ItemCreated Handle(CreateItemCommand command, CatalogDbContext db)
    {
        var item = new Item { Name = command.Name };

        db.Items.Add(item);

        return new ItemCreated(item.Id);
    }
}

public class ItemCreatedHandler
{
    public void Handle(ItemCreated @event)
    {
        Console.WriteLine("You created a new item with id " + @event.Id);
    }
}

public static class ItemEndpoints
{
    [Transactional]
    [WolverinePost("/api/items/create"), AlwaysPublishResponse]
    public static (ItemCreated, Insert<Item>) Post(CreateItemCommand command)
    {
        // Create a new Item entity
        var item = new Item
        {
            Name = command.Name,
            Id = CombGuidIdGeneration.NewGuid(),
            Description = string.Empty
        };

        return (new ItemCreated(item.Id), Storage.Insert(item));
    }

    [WolverineGet("/api/item/{id}")]
    public static Item Get([Entity] Item item) => item;
}
