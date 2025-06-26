using Marten;
using Marten.Schema;

namespace Inventory.Api.Documents;

public class ProcurementOrdersInitialData : IInitialData
{
    private readonly object[] _initialData;

    public ProcurementOrdersInitialData(params object[] initialData)
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

public static class ProcurementOrdersDatasets
{
    public static readonly ProcurementOrder[] Vendors =
    {
        new() { Id = 1001, VendorId = 101 },
        new() { Id = 1002, VendorId = 102 },
    };
}
