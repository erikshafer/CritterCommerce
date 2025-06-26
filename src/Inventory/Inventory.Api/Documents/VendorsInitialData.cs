using Marten;
using Marten.Schema;

namespace Inventory.Api.Documents;

public class VendorsInitialData : IInitialData
{
    private readonly object[] _initialData;

    public VendorsInitialData(params object[] initialData)
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

public static class VendorsDatasets
{
    public static readonly Vendor[] Vendors =
    {
        new() { Id = 101, Name = "Acme Corp." },
        new() { Id = 102, Name = "Stark Industries" },
        new() { Id = 103, Name = "Dunder Mifflin" },
        new() { Id = 104, Name = "Initech" },
    };
}
