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

        var streamId1 = Guid.CreateVersion7();
        var inv1 = session.Events.StartStream<Inventory>(streamId1, new InventoryInitialized(streamId1, sku1));
        var streamId2 = Guid.CreateVersion7();
        var inv2 = session.Events.StartStream<Inventory>(streamId1, new InventoryInitialized(streamId2, sku2));
        var streamId3 = Guid.CreateVersion7();
        var inv3 = session.Events.StartStream<Inventory>(streamId1, new InventoryInitialized(streamId3, sku3));

        await session.SaveChangesAsync();

        session.Events.Append(streamId1, new InventoryMarkedReady());
        session.Events.Append(streamId2, new InventoryMarkedReady());
        session.Events.Append(streamId3, new InventoryMarkedReady());

        await session.SaveChangesAsync();

        session.Events.Append(streamId1, new InventoryDecremented(1));
        session.Events.Append(streamId1, new InventoryDecremented(1)); // 2
        session.Events.Append(streamId1, new InventoryDecremented(1)); // 3
        session.Events.Append(streamId1, new InventoryDecremented(2)); // 5
        session.Events.Append(streamId1, new InventoryDecremented(1)); // 6
        session.Events.Append(streamId1, new InventoryDecremented(2)); // 8
        session.Events.Append(streamId1, new InventoryDecremented(1)); // 9
        session.Events.Append(streamId1, new InventoryDecremented(1)); // 10

        session.Events.Append(streamId2, new InventoryDecremented(1));
        session.Events.Append(streamId2, new InventoryDecremented(1));

        await session.SaveChangesAsync();

        session.Events.Append(streamId1, new InventoryIncremented(2)); // should be 20, not 2, resulting in...

        await session.SaveChangesAsync();

        return true;
    }
}
