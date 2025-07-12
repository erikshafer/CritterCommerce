namespace Inventory.Api.WarehouseLevels;

public sealed record InventoryLevelAdjustmentMessage(
    Guid InventoryLevelId,
    int Quantity
);

public sealed record InventoryLevelMovedLotsMessage(
    Guid InventoryLevelId,
    string OldFacilityLotId,
    string NewFacilityLotId
);
