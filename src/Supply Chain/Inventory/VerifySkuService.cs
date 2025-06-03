namespace Inventory;

public sealed record SkuView(string Value);

public interface IVerifySkuService
{
    Task<bool> VerifySkuIsNotInUse(string sku);
}

public sealed class VerifySkuService : IVerifySkuService
{
    public async Task<bool> VerifySkuIsNotInUse(string sku)
    {
        return true;
    }
}
