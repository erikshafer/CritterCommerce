using Marten;
using Marten.Events.Projections;

namespace Catalog.DraftTaxonomy;

internal static class Config
{
    internal static void AddDraftCategoryProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<DraftCategory>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id);

        // projections

        // views
    }

    internal static void AddDraftCategoryServices(this IServiceCollection services)
    {

    }
}
