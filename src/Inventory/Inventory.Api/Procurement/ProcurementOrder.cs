namespace Inventory.Api.Procurement;

public class ProcurementOrder
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = null!;
    public int VendorId { get; set; }
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public DateTime? RecordedAt { get; set; }
    public DateTime? OrderedAt { get; set; }
}
