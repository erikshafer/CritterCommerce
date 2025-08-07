using Legacy.Catalog.Application;
using Legacy.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;

namespace Legacy.Catalog.Api.Items;

public static class QueryItemEndpoints
{
    [WolverineGet("/api/items/{id:guid}", Name = "GetItemById")]
    public static async Task<Item> GetItemById(int id, CatalogDbContext db) =>
        await db.Items.AsNoTracking().FirstAsync(x => x.Id == id);

    [WolverineGet("/api/items", Name = "GetAllItems")]
    public static async Task<IReadOnlyList<Item>> GetAllItems(CatalogDbContext db) =>
        await db.Items.AsNoTracking().ToListAsync();
}
