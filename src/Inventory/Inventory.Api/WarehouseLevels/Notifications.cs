namespace Inventory.Api.WarehouseLevels;

public record InventoryLevelAdjustmentNotification(Guid InventoryLevelId, int Quantity);
