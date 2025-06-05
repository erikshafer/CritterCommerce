using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Inventory.Api;

public static class QueryInventoryEndpoints
{
    [WolverineGet("/api/inventory", Name = "All InventoryItems")]
    public static async Task<IReadOnlyList<InventoryItem>> GetAllDomainModels(IDocumentSession session) =>
        await session.Query<InventoryItem>().ToListAsync();



    [WolverineGet("/api/inventory/{id}", Name = "InventoryItem domain model (inline)")]
    public static InventoryItem GetDomainModelById(Guid id, [ReadAggregate] InventoryItem inventoryItem) => inventoryItem;



    [WolverineGet("/api/inventory/live-aggregate/{id}",  Name = "Live Aggregation of InventoryItem")]
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



    [WolverineGet("/api/inventory/sku", Name = "All Inventory SKU read models")]
    public static async Task<IReadOnlyList<InventorySku>> GetAllInventorySkuReadModels(IDocumentSession session) =>
        await session.Query<InventorySku>().ToListAsync();



    [WolverineGet("/api/inventory/sku/stream-id/{id}",  Name = "Inventory SKU read model by stream ID 1")]
    [ProducesResponseType(404)]
    [ProducesResponseType(200, Type = typeof(InventorySku))]
    public static async Task<IResult> GetInventorySkuReadModelByStreamId1(
        Guid id,
        IQuerySession session,
        CancellationToken ct)
    {
        var readModel = await session.LoadAsync<InventorySku>(id, ct);
        return readModel == null
            ? Results.NotFound()
            : Results.Ok(readModel);
    }



    // [WolverineGet("/api/inventory/sku/stream-id/{id}",  Name = "Inventory SKU read model by stream ID 2")]
    // public static InventorySku GetInventorySkuReadModelByStreamId2([Document] InventorySku invoice) => invoice;



    [WolverineGet("/api/inventory/sku/{sku}", Name = "Inventory SKU by SKU")]
    public static async Task<InventorySku> GetInventorySkuReadModelBySku(string sku, IDocumentSession session) =>
        await session.Query<InventorySku>().FirstAsync(x => x.Sku == sku);
}
