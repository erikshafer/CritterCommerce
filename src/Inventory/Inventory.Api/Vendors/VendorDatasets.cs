namespace Inventory.Api.Vendors;

public static class VendorsDatasets
{
    public static readonly Vendor[] Data =
    {
        new() { Id = 101, Name = "Acme Corp." },
        new() { Id = 102, Name = "Stark Industries" },
        new() { Id = 103, Name = "Dunder Mifflin" },
        new() { Id = 104, Name = "Initech" },
    };
}
