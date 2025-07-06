using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseInventories.Endpoints;

public static class QueryInventoryEndpoints
{
    [WolverineGet("/api/inventory", Name = "All InventoryItems"), Tags(Tags.WarehouseInventories)]
    public static async Task<IReadOnlyList<InventoryItem>> GetAllDomainModels(IDocumentSession session) =>
        await session.Query<InventoryItem>().ToListAsync();

    [WolverineGet("/api/inventory/{id}", Name = "InventoryItem domain model (inline)"), Tags(Tags.WarehouseInventories)]
    public static InventoryItem GetDomainModelById(Guid id, [ReadAggregate] InventoryItem inventoryItem) =>
        inventoryItem;

    [WolverineGet("/api/inventory/live-aggregate/{id}", Name = "Live Aggregation of InventoryItem"), Tags(Tags.WarehouseInventories)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200, Type = typeof(InventoryItem))]
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
