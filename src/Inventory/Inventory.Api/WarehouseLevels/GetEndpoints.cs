using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseLevels;

public static class QueryEndpoints
{
    [Tags(Tags.WarehouseInventoryLevels)]
    [WolverineGet("/api/inventory-level", Name = "All InventoryItems")]
    public static async Task<IReadOnlyList<InventoryLevel>> GetAllDomainModels(IDocumentSession session) =>
        await session.Query<InventoryLevel>().ToListAsync();

    [Tags(Tags.WarehouseInventoryLevels)]
    [WolverineGet("/api/inventory-level/{id}", Name = "InventoryItem domain model (inline)")]
    public static InventoryLevel GetDomainModelById(Guid id, [ReadAggregate] InventoryLevel inventoryLevel) =>
        inventoryLevel;

    [Tags(Tags.WarehouseInventoryLevels)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200, Type = typeof(InventoryLevel))]
    [WolverineGet("/api/inventory-level/live-aggregate/{id}", Name = "Live Aggregation of InventoryItem")]
    public static async Task<IResult> GetLiveAggregationOfDomainModel(
        Guid id,
        IQuerySession session,
        CancellationToken ct)
    {
        var aggregatedInventoryStream = await session.Events.AggregateStreamAsync<InventoryLevel>(id, token: ct);
        return aggregatedInventoryStream == null
            ? Results.NotFound()
            : Results.Ok(aggregatedInventoryStream);
    }
}
