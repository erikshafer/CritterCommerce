namespace Catalog.Api.Items;

public sealed record AssignSkuToDraftItem(int Sku, Guid ItemId);

public sealed record AssignedSkuToDraftItem(int Sku, Guid ItemId);

public static class AssignSkuToDraftItemHandler
{
    public static async Task<AssignedSkuToDraftItem> Handle(AssignSkuToDraftItem command, CatalogDbContext db)
    {
        var skuReservation = await db.SkuReservations.FindAsync(command.Sku);
        var draftItem = await db.Items.FindAsync(command.ItemId);

        if (skuReservation is null || draftItem is null)
            throw new Exception("Something is null, WTF?");

        // <business logic>

        var assignment = await db.SkuItemAssignments.FindAsync(command.Sku);

        if (assignment is null)
        {
            var assign = new SkuItemAssignment
            {
                Sku = skuReservation.Unit,
                ItemId = draftItem.Id
            };
            await db.SkuItemAssignments.AddAsync(assign);
        }

        if (assignment is not null)
        {
            // new item id assigned
            assignment.ItemId = draftItem.Id;
        }

        return new AssignedSkuToDraftItem(skuReservation.Unit, draftItem.Id);
    }
}
