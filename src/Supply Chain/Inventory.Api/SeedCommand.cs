using JasperFx.CommandLine;
using Marten;

namespace Inventory.Api;

[Description("Set up some inventory streams and events")]
public class SeedCommand : JasperFxAsyncCommand<NetCoreInput>
{
    public override async Task<bool> Execute(NetCoreInput input)
    {
        using var host = input.BuildHost();
        using var scope = host.Services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

        await store.Advanced.Clean.DeleteAllDocumentsAsync();
        await store.Advanced.Clean.DeleteAllEventDataAsync();

        await using var session = store.LightweightSession();

        const string sku1 = "11001";
        const string sku2 = "11002";
        const string sku3 = "11003";

        var inv1 = session.Events.StartStream<Inventory>(new InventoryInitialized(sku1)).Id;
        var inv2 = session.Events.StartStream<Inventory>(new InventoryInitialized(sku2)).Id;
        var inv3 = session.Events.StartStream<Inventory>(new InventoryInitialized(sku3)).Id;

        await session.SaveChangesAsync();

        session.Events.Append(inv1, new InventoryValidatedForUse());
        session.Events.Append(inv2, new InventoryValidatedForUse());
        session.Events.Append(inv3, new InventoryValidatedForUse());

        await session.SaveChangesAsync();

        session.Events.Append(inv1, new InventoryDecremented(1));
        session.Events.Append(inv1, new InventoryDecremented(1)); // 2
        session.Events.Append(inv1, new InventoryDecremented(1)); // 3
        session.Events.Append(inv1, new InventoryDecremented(2)); // 5
        session.Events.Append(inv1, new InventoryDecremented(1)); // 6
        session.Events.Append(inv1, new InventoryDecremented(2)); // 8
        session.Events.Append(inv1, new InventoryDecremented(1)); // 9
        session.Events.Append(inv1, new InventoryDecremented(1)); // 10

        session.Events.Append(inv2, new InventoryDecremented(1));
        session.Events.Append(inv2, new InventoryDecremented(1));

        await session.SaveChangesAsync();

        session.Events.Append(inv1, new InventoryIncremented(2)); // should be 20, not 2, resulting in...

        await session.SaveChangesAsync();

        session.Events.Append(inv1, new PhysicalInventoryCountCorrection(110)); // ... a correction!

        await session.SaveChangesAsync();

        return true;
    }
}
