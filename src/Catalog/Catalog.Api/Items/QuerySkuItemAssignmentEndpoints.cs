using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;

namespace Catalog.Api.Items;

public static class QuerySkuItemAssignmentEndpoints
{
    [WolverineGet("/api/sku-item-assignments/{id}", Name = "GetSkuItemAssignment")]
    public static SkuItemAssignment Get([Entity] SkuItemAssignment assignment) => assignment;

    [WolverineGet("/api/sku-item-assignments", Name = "GetSkuItemAssignments")]
    public static async Task<IReadOnlyList<SkuItemAssignment>> GetAssignments(CatalogDbContext db) =>
        await db.SkuItemAssignments.AsNoTracking().ToListAsync();
}
