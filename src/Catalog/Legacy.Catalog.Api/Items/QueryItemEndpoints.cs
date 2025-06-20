using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;

namespace Legacy.Catalog.Api.Items;

public static class QueryItemEndpoints
{
    [WolverineGet("/api/items/{id:guid}", Name = "GetItem")]
    public static Item Get([Entity] Item item) => item;

    [WolverineGet("/api/items", Name = "GetItems")]
    public static async Task<IReadOnlyList<Item>> GetItems(CatalogDbContext db) =>
        await db.Items.AsNoTracking().ToListAsync();
}
