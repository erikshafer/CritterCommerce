using Inventory.Api.Locations;
using Inventory.Api.Vendors;
using Marten;
using Marten.Schema;

namespace Inventory.Api;

public class InitialInventoryDataSet : IInitialData
{
    private readonly object[] _initialData;

    public InitialInventoryDataSet(params object[] initialData)
    {
        _initialData = initialData;
    }

    public async Task Populate(IDocumentStore store, CancellationToken ct)
    {
        await using var dirty = store.DirtyTrackedSession();
        await dirty.DocumentStore.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(Location), ct);
        await dirty.DocumentStore.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(Vendor), ct);
        dirty.Store(_initialData);
        await dirty.SaveChangesAsync(ct);
    }

    public static object[] ConcatDataSets(params object[][] dataSets) =>
        dataSets.SelectMany(x => x).ToArray();
}
