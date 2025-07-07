using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseInventories;

public static class QueryEndpoints
{
    [Tags(Tags.WarehouseInventories)]
    [WolverineGet("/api/inventory", Name = "All InventoryItems")]
    public static async Task<IReadOnlyList<InventoryItem>> GetAllDomainModels(IDocumentSession session) =>
        await session.Query<InventoryItem>().ToListAsync();

    [Tags(Tags.WarehouseInventories)]
    [WolverineGet("/api/inventory/{id}", Name = "InventoryItem domain model (inline)")]
    public static InventoryItem GetDomainModelById(Guid id, [ReadAggregate] InventoryItem inventoryItem) =>
        inventoryItem;

    [Tags(Tags.WarehouseInventories)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200, Type = typeof(InventoryItem))]
    [WolverineGet("/api/inventory/live-aggregate/{id}", Name = "Live Aggregation of InventoryItem")]
    public static async Task<IResult> GetLiveAggregationOfDomainModel(
        Guid id,
        IQuerySession session,
        CancellationToken ct)
    {
        var aggregatedInventoryStream = await session.Events.AggregateStreamAsync<InventoryItem>(id, token: ct);
        return aggregatedInventoryStream == null
            ? Results.NotFound()
            : Results.Ok(aggregatedInventoryStream);
    }
}
