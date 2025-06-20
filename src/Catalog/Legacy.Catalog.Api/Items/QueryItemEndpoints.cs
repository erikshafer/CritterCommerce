using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Legacy.Catalog.Api.Items;

public static class QueryItemEndpoints
{
    [WolverineGet("/api/items/{id:guid}", Name = "GetItemById")]
    public static async Task<Item> Get(Guid id, CatalogDbContext db) =>
        await db.Items.AsNoTracking().FirstAsync(x => x.Id == id);

    [WolverineGet("/api/items", Name = "GetAllItems")]
    public static async Task<IReadOnlyList<Item>> GetItems(CatalogDbContext db) =>
        await db.Items.AsNoTracking().ToListAsync();
}
