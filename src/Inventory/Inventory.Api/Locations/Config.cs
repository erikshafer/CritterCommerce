using Marten;

namespace Inventory.Api.Locations;

internal static class Config
{
    internal static void AddLocationDocuments(this StoreOptions opts)
    {
        opts.RegisterDocumentType<Location>();
        opts.Schema.For<Location>()
            .Identity(x => x.Id)
            .Duplicate(x => x.Name);
    }
}
