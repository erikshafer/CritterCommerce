namespace Inventory.Api.Documents;

public class ProcurementOrder
{
    public int Id { get; set; }
    public string ShipmentNumber => Id.ToString();
    public int VendorId { get; set; }
}
