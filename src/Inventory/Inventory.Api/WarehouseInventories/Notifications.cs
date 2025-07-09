namespace Inventory.Api.WarehouseInventories;

/// <summary>
/// Event notification message
/// </summary>
public record InventoryAdjustmentNotification(Guid InventoryId, int Quantity);
