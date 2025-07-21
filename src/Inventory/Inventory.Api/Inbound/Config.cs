using Inventory.Api.Inbound.Views;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;

namespace Inventory.Api.Inbound;

internal static class Config
{
    internal static void AddFreightProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<FreightShipment>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Origin)
            .Duplicate(x => x.Destination);

        opts.Projections.Add<DailyShipmentsProjection>(ProjectionLifecycle.Async);
        opts.Projections.Add<FreightShipmentProjection>(ProjectionLifecycle.Async);
    }
}
