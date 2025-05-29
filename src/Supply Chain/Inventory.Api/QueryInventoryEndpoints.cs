using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Persistence;

namespace Inventory.Api;

public static class QueryInventoryEndpoints
{
    [WolverineGet("/api/inventory/{id}", Name = "GetInventory")]
    public static Inventory GetEntity([Entity] Inventory inventory) => inventory;

    [WolverineGet("/api/inventory/read-model/{id}",  Name = "GetInventoryReadModel")]
    [ProducesResponseType(404)]
    [ProducesResponseType(200, Type = typeof(InventoryReadModel))]
    public static async Task<IResult> GetReadModel(
        Guid id,
        IQuerySession session,
        CancellationToken ct)
    {
        var invoice = await session.LoadAsync<InventoryReadModel>(id, ct);
        return invoice == null
            ? Results.NotFound()
            : Results.Ok(invoice);
    }
}
