namespace Inventory.Api.WarehouseLevels;

public interface IFacilityLotService
{
    bool IsLotAvailable(string lotNumber);
}

public class FacilityLotService : IFacilityLotService
{
    public bool IsLotAvailable(string lotNumber)
    {
        return true;
    }
}

