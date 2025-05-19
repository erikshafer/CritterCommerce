using Marten;
using Wolverine.Http;
using Wolverine.Persistence;

namespace Inventory.Api;

public static class QueryInventoryEndpoints
{
    [WolverineGet("/api/inventory/{id}", Name = "GetInventory")]
    public static Inventory Get([Entity] Inventory inventory) => inventory;

    [WolverineGet("/api/inventory/inline-projections", Name = "GetInventoryInlineProjections")]
    public static async Task<IReadOnlyList<InventoryProjection>> GetInventoryProjections(IQuerySession session) =>
        session.Query<InventoryProjection>().ToList();
}
