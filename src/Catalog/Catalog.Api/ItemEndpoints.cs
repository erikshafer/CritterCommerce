using JasperFx.Core;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Persistence;

namespace Catalog.Api;

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
