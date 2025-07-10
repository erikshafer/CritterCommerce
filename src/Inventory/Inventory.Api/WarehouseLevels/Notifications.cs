namespace Inventory.Api.WarehouseLevels;

public record InventoryAdjustmentNotification(Guid ItemInventoryId, int Quantity);
