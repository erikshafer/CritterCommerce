using Inventory.Api.Receiving.Views;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;

namespace Inventory.Api.Receiving;

internal static class Config
{
    internal static void AddReceivingProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<ReceivedShipment>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id);

        opts.Projections.Add<ExpectedQuantityAnticipatedProjection>(ProjectionLifecycle.Async);
    }
}
