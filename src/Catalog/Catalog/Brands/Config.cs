using Marten;
using Marten.Events.Projections;

namespace Catalog.Brands;

internal static class Config
{
    internal static void AddBrandProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<Brand>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Name);

        // projections

        // views
    }

    internal static void AddBrandServices(this IServiceCollection services)
    {

    }
}
