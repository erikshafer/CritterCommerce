namespace Inventory.Api.WarehouseLevels;

public enum InventoryLevelStatus
{
    Available = 1,
    CriticalHold = 2,
    Damaged = 3,
    Expired = 4,
    Hold = 5,
    Inspection = 6,
    PutAside = 7,
    Release = 8,
    Returned = 9,
    Salvaged = 10,
    TimeHold = 11,
    OffQuality = 12,
    Unshippable = 13,
    Unusable = 14
}
