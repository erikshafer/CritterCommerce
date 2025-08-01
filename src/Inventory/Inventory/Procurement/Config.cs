using Marten;

namespace Inventory.Procurement;

internal static class Config
{
    internal static void AddProcurementDocuments(this StoreOptions opts)
    {
        opts.RegisterDocumentType<ProcurementOrder>();
        opts.Schema.For<ProcurementOrder>()
            .Identity(x => x.Id)
            .Duplicate(x => x.VendorId) // Consider making this a foreign key to the Vendor docs
            .Duplicate(x => x.TrackingNumber); // Could add the entire document's properties here, but
    }
}
