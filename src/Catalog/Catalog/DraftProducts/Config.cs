using Catalog.DraftProducts.Services;
using Marten;
using Marten.Events.Projections;

namespace Catalog.DraftProducts;

internal static class Config
{
    internal static void AddDraftProductProjections(this StoreOptions opts)
    {
        opts.Projections
            .Snapshot<DraftProduct>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id);

        // projections

        // views
    }

    internal static void AddDraftProductServices(this IServiceCollection services)
    {
        services.AddSingleton<ISkuService, SkuService>();
        services.AddSingleton<IBrandService, BrandService>();
        services.AddSingleton<ICategoryLookupService, CategoryLookupService>();
    }
}
