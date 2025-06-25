using Marten;
using Marten.Schema;

namespace Inventory.Locations;

public class LocationsInitialData : IInitialData
{
    private readonly object[] _initialData;

    public LocationsInitialData(params object[] initialData)
    {
        _initialData = initialData;
    }

    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        await using var session = store.LightweightSession();
        // Marten UPSERT will cater for existing records
        session.Store(_initialData);
        await session.SaveChangesAsync(cancellation);
    }
}
