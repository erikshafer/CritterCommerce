using Marten;
using Marten.Events.Projections;

namespace Catalog.Products;

internal static class Config
{
    internal static void AddProductProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<Product>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Sku)
            .Duplicate(x => x.BrandId);

        // projections

        // views
    }

    internal static void AddProductServices(this IServiceCollection services)
    {

    }
}
