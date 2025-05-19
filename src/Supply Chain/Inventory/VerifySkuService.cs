using Marten;

namespace Inventory;

public sealed record SkuView(string Value);

public interface IVerifySkuService
{
    Task<InventoryInitialized> Initialize(Sku sku, IQuerySession session);
    Task<bool> VerifySkuIsNotInUse(Sku sku);
}

public sealed class VerifySkuService : IVerifySkuService
{
    public async Task<InventoryInitialized> Initialize(Sku sku, IQuerySession session)
    {
        var verifiedSku = await session.LoadAsync<SkuView>(sku.Value);

        return new InventoryInitialized(sku.Value);
    }

    public async Task<bool> VerifySkuIsNotInUse(Sku sku)
    {
        return true;
    }
}
