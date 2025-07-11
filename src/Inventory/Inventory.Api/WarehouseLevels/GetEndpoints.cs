using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseLevels;

public static class QueryEndpoints
{
    [Tags(Tags.WarehouseInventoryLevels)]
    [WolverineGet("/api/inventory-level", Name = "All")]
    public static async Task<IReadOnlyList<InventoryLevel>> GetAllDomainModels(IDocumentSession session) =>
        await session.Query<InventoryLevel>().ToListAsync();

    [Tags(Tags.WarehouseInventoryLevels)]
    [WolverineGet("/api/inventory-level/{id}", Name = "Inline projection")]
    public static InventoryLevel GetDomainModelById(Guid id, [ReadAggregate] InventoryLevel inventoryLevel) =>
        inventoryLevel;

    [Tags(Tags.WarehouseInventoryLevels)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200, Type = typeof(InventoryLevel))]
    [WolverineGet("/api/inventory-level/live-aggregate/{id}", Name = "Live aggregation")]
    public static async Task<IResult> GetLiveAggregation(
        Guid id,
        IQuerySession session,
        CancellationToken ct)
    {
        var aggregatedInventoryStream = await session.Events.AggregateStreamAsync<InventoryLevel>(id, token: ct);
        return aggregatedInventoryStream == null
            ? Results.NotFound()
            : Results.Ok(aggregatedInventoryStream);
    }

    [Tags(Tags.WarehouseInventoryLevels)]
    [ProducesResponseType(404)]
    [ProducesResponseType(200, Type = typeof(InventoryLevel))]
    [WolverineGet("/api/inventory-level/live-aggregate/{id}/version/{version}", Name = "Live aggregation with version")]
    public static async Task<IResult> GetWithVersion(
        Guid id,
        [FromRoute] int version,
        IQuerySession session,
        CancellationToken ct)
    {
        var aggregatedInventoryStream = await session.Events.AggregateStreamAsync<InventoryLevel>(id, version, token: ct);
        return aggregatedInventoryStream == null
            ? Results.NotFound()
            : Results.Ok(aggregatedInventoryStream);
    }
}
