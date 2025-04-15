using Wolverine.Attributes;

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
