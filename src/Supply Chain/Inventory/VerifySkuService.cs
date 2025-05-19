using Marten;

namespace Inventory;

public sealed record SkuView(string Value);

public interface IVerifySkuService
{
    Task<InventoryInitialized> Initialize(Sku sku, IQuerySession session);
}

public sealed class VerifySkuService : IVerifySkuService
{
    public async Task<InventoryInitialized> Initialize(Sku sku, IQuerySession session)
    {
        var verifiedSku = await session.LoadAsync<SkuView>(sku.Value);

        return new InventoryInitialized(sku.Value);
    }
}
