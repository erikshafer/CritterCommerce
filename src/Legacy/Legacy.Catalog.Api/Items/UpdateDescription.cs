using Legacy.Catalog.Application;
using Wolverine.Http;

namespace Legacy.Catalog.Api.Items;

public sealed record UpdateDescription(Guid Id, string Description);

public sealed record DescriptionUpdated(Guid Id, bool Accepted);

public sealed record DescriptionUpdatedResponse(Guid Id, bool Accepted)
    : AcceptResponse("/api/items/" + Id);

public static class UpdateDescriptionEndpoint
{
    [WolverinePost("/api/items/description", Name = "UpdateDescription")]
    public static (DescriptionUpdatedResponse, DescriptionUpdated) Handle(
        UpdateDescription command,
        CatalogDbContext db)
    {
        var item = db.Items.Find(command.Id);

        if (item == null)
            return (
                new DescriptionUpdatedResponse(command.Id, false),
                new DescriptionUpdated(command.Id, false)
            );

        item.Description = command.Description;

        return (
            new DescriptionUpdatedResponse(item.Id, true),
            new DescriptionUpdated(item.Id, true));
    }
}

public class DescriptionUpdatedHandler
{
    public void Handle(DescriptionUpdated @event)
    {
        Console.WriteLine("You updated the description of an item with id {0}", @event.Id);
    }
}
