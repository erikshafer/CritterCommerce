using Marten;

namespace Inventory.Api.Vendors;

internal static class Config
{
    internal static void AddVendorDocuments(this StoreOptions opts)
    {
        opts.RegisterDocumentType<Vendor>();
        opts.Schema.For<Vendor>()
            .Identity(x => x.Id)
            .Duplicate(x => x.Name);
    }
}
