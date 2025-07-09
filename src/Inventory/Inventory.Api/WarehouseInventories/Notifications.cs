namespace Inventory.Api.WarehouseInventories;

public record InventoryAdjustmentNotification(Guid ItemInventoryId, int Quantity);
