namespace Inventory.Api.ReceivingShipments;

public sealed record LineItem
{
    public string Sku { get; set; } = default!;
    public int ExpectedQuantity { get; set; }
    public int? ReceivedQuantity { get; set; }
}
