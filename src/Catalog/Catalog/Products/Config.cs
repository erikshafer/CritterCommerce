using Marten;
using Marten.Events.Projections;

namespace Catalog.Products;

internal static class Config
{
    internal static void AddProductProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<DraftProduct>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Sku);

        // projections

        // views
    }

    internal static void AddProductServices(this IServiceCollection services)
    {

    }
}
