using Inventory.Api.WarehouseInventories;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseLevels;

public static class QueryEndpoints
{
    [Tags(Tags.WarehouseInventories)]
    [WolverineGet("/api/warehouse-inventories", Name = "All InventoryItems")]
    public static async Task<IReadOnlyList<ItemInventory>> GetAllDomainModels(IDocumentSession session) =>
        await session.Query<ItemInventory>().ToListAsync();

    [Tags(Tags.WarehouseInventories)]
    [WolverineGet("/api/warehouse-inventories/{id}", Name = "InventoryItem domain model (inline)")]
    public static ItemInventory GetDomainModelById(Guid id, [ReadAggregate] ItemInventory itemInventory) =>
        itemInventory;

    [Tags(Tags.WarehouseInventories)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200, Type = typeof(ItemInventory))]
    [WolverineGet("/api/warehouse-inventories/live-aggregate/{id}", Name = "Live Aggregation of InventoryItem")]
    public static async Task<IResult> GetLiveAggregationOfDomainModel(
        Guid id,
        IQuerySession session,
        CancellationToken ct)
    {
        var aggregatedInventoryStream = await session.Events.AggregateStreamAsync<ItemInventory>(id, token: ct);
        return aggregatedInventoryStream == null
            ? Results.NotFound()
            : Results.Ok(aggregatedInventoryStream);
    }
}
