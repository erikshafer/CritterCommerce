namespace Inventory.Api.ReceivingShipments.Services;

public interface ISkuService
{
    bool DoesSkuExist(string sku);
}

public class SkuService : ISkuService
{
    public bool DoesSkuExist(string sku)
    {
        return true;
    }
}

