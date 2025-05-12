namespace Inventory;

public class Inventory
{
    public Guid Id { get; set; }
    public Quantity Quantity { get; set; }
    public InventoryStatus Status { get; set; }
}
