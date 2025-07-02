using Marten;
using Marten.Schema;

namespace Inventory.Api.Vendors;

public class VendorsInitialData : IInitialData
{
    private readonly object[] _initialData;

    public VendorsInitialData(params object[] initialData)
    {
        _initialData = initialData;
    }

    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        await using var dirty = store.DirtyTrackedSession();
        await dirty.DocumentStore.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(Vendor), cancellation);

        dirty.Store(_initialData);
        await dirty.SaveChangesAsync(cancellation);
    }
}
