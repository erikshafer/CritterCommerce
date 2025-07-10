namespace Inventory.Api.WarehouseLevels;

public sealed record InventoryLevelAdjustmentNotification(
    Guid InventoryLevelId,
    int Quantity
);

public sealed record InventoryLevelMovedLots(
    Guid InventoryLevelId,
    string OldFacilityLotId,
    string NewFacilityLotId
);
