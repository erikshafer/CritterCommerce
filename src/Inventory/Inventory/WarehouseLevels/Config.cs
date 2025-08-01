using Inventory.WarehouseLevels.Lots;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;

namespace Inventory.WarehouseLevels;

internal static class Config
{
    internal static void AddWarehouseProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<InventoryLevel>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Sku);

        opts.Projections.Add<WarehouseLotsProjection>(ProjectionLifecycle.Async);

        opts.Schema.For<WarehouseLotsView>()
            .Duplicate(x => x.Warehouse)
            .Duplicate(x => x.Lot);
    }

    internal static void AddWarehouseServices(this IServiceCollection services)
    {
        services.AddSingleton<IFacilityLotService, FacilityLotService>();
    }
}
