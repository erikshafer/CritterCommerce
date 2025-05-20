using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Persistence;

namespace Inventory.Api;

public static class QueryInventoryEndpoints
{
    [WolverineGet("/api/inventory/{id}", Name = "GetInventory")]
    public static Inventory Get([Entity] Inventory inventory) =>
        inventory;

    // [WolverineGet("/api/inventory/inline-projections/{id}", Name = "GetInventoryInlineProjection")]
    [WolverineGet("/api/inventory/inline-projections/{id}")]
    [ProducesResponseType(404)]
    [ProducesResponseType(200, Type = typeof(InventoryProjection))]
    public static async Task<IResult> GetInventoryProjection(Guid id, IQuerySession session, CancellationToken ct)
    {
        var inventory = await session.LoadAsync<InventoryProjection>(id, ct);
        return inventory == null
            ? Results.NotFound()
            : Results.Ok(inventory);
    }

    [WolverineGet("/api/inventory/inline-projections", Name = "GetInventoryInlineProjections")]
    public static async Task<IReadOnlyList<InventoryProjection>> GetAllInventoryProjections(IQuerySession session) =>
        session.Query<InventoryProjection>().ToList();
}
