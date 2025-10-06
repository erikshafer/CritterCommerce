using Marten;
using Marten.Events.Projections;

namespace Catalog.Taxonomy;

internal static class Config
{
    internal static void AddCategoryProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<DraftCategory>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id);

        // projections

        // views
    }

    internal static void AddCategoryServices(this IServiceCollection services)
    {

    }
}
