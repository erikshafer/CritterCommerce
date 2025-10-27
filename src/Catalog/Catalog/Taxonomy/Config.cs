using Marten;
using Marten.Events.Projections;

namespace Catalog.Taxonomy;

internal static class Config
{
    internal static void AddCategoryProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<Category>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.ParentId)
            .Duplicate(x => x.Code);

        // projections

        // views
    }

    internal static void AddCategoryServices(this IServiceCollection services)
    {

    }
}
