using JasperFx.Core;

namespace Inventory.Locations;

public static class LocationsDatasets
{
    public static Location[] Data =>
        MapFulfillmentCentersToLocations
            .Concat(SupplierWarehouseLocations)
            .ToArray();

    private static readonly Location[] MapFulfillmentCentersToLocations =
        FulfillmentCenters.FulfillmentCenter.List().Select(source =>
                new Location
                {
                    Id = CombGuidIdGeneration.NewGuid(),
                    Name = source.Name,
                    Code = source.Shorthand
                })
            .ToArray();

    private static readonly Location[] SupplierWarehouseLocations =
    {
        new() { Id = Guid.Parse("2219b6f7-7883-4629-95d5-1a8a6c74b244"), Name = "Acme Corp." },
        new() { Id = Guid.Parse("331c15b4-b7bd-44d6-a804-b6879f99a65f"), Name = "Stark Industries" },
        new() { Id = Guid.Parse("9cdc4397-0624-4f4f-bc2f-b439fa6b85f0"), Name = "Dunder Mifflin" },
        new() { Id = Guid.Parse("432c5bed-75f0-4aea-9f12-780cd4b4e2c1"), Name = "Initech" },
        new() { Id = Guid.Parse("642a3e95-5875-498e-8ca0-93639ddfebcd"), Name = "Wonka Industries" },
        new() { Id = Guid.Parse("9d8ef25a-de9a-41e5-b72b-13f24b735883"), Name = "Gekko & Co" },
        new() { Id = Guid.Parse("0de5e136-7926-42b5-b1b1-552e7c8a435c"), Name = "Cyberdyne Systems" },
        new() { Id = Guid.Parse("953c2b2c-9cdd-498c-b08d-f31220e16d05"), Name = "Sterling Cooper" },
        new() { Id = Guid.Parse("16de7fb9-f8ed-47d7-84fe-6ce02c432557"), Name = "Charming Essentialz" }
    };
}
