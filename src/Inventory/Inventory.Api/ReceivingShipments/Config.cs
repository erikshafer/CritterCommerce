using Marten;
using Marten.Events.Projections;

namespace Inventory.Api.ReceivingShipments;

internal static class Config
{
    internal static void AddReceivingShipmentsProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<ReceivedShipment>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id);
    }

    internal static void AddReceivingShipmentsServices(this IServiceCollection services)
    {
        // Register related services here if needed in the future
    }
}
