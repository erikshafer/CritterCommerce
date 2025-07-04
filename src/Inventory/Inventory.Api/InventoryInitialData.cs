using Inventory.Api.Locations;
using Inventory.Api.Procurement;
using Inventory.Api.Vendors;
using Marten;
using Marten.Schema;

namespace Inventory.Api;

public class InventoryInitialData : IInitialData
{
    private readonly object[] _initialData;

    public InventoryInitialData(params object[] initialData)
    {
        _initialData = initialData;
    }

    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        await using var dirty = store.DirtyTrackedSession();
        await dirty.DocumentStore.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(Location), cancellation);
        await dirty.DocumentStore.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(Vendor), cancellation);
        await dirty.DocumentStore.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(ReceivedProcurementOrder), cancellation);
        dirty.Store(_initialData);
        await dirty.SaveChangesAsync(cancellation);
    }

    public static object[] ConcatDataSets(params object[][] dataSets) =>
        dataSets.SelectMany(x => x).ToArray();
}
