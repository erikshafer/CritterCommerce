namespace Inventory.Api.WarehouseInventories;

/// <summary>
/// Event notification message
/// </summary>
public record InventoryAdjustmentCompleted(Guid InventoryId, int Quantity);
