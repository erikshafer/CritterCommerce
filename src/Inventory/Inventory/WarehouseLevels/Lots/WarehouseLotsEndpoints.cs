using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Inventory.WarehouseLevels.Lots;

public static class WarehouseLotsEndpoints
{
    [Tags(Tags.WarehouseInventoryLevels)]
    [WolverineGet("/api/inventory-level/warehouse-lots")]
    public static async Task<IReadOnlyList<WarehouseLotsView>> GetAll(
        IQuerySession session) =>
        await session
            .Query<WarehouseLotsView>()
            .OrderBy(x => x.Warehouse)
            .ToListAsync();

    [Tags(Tags.WarehouseInventoryLevels)]
    [WolverineGet("/api/inventory-level/warehouse-lots/{warehouse}")]
    public static async Task<IReadOnlyList<WarehouseLotsView>> GetByWarehouse(
        IQuerySession session,
        [FromRoute] string warehouse) =>
        await session
            .Query<WarehouseLotsView>()
            .Where(x => x.Warehouse == warehouse)
            .OrderBy(x => x.Lot)
            .ToListAsync();
}
