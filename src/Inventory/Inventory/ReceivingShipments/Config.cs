using Inventory.ReceivingShipments.Services;
using Inventory.ReceivingShipments.Views;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;

namespace Inventory.ReceivingShipments;

internal static class Config
{
    internal static void AddReceivingShipmentsProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<ReceivedShipment>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id);

        opts.Projections.Add<ExpectedQuantityAnticipatedProjection>(ProjectionLifecycle.Async);

        opts.Projections.Add<DailyShipmentsDeliveredProjection>(ProjectionLifecycle.Async);

        opts.Projections.Add<OutstandingReceivedShipmentsProjection>(ProjectionLifecycle.Async);
    }

    internal static void AddReceivingShipmentsServices(this IServiceCollection services)
    {
        services.AddSingleton<ISkuService, SkuService>();
    }
}
