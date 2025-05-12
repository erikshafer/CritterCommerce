using JasperFx.CommandLine;
using Marten;

namespace Inventory.Api;

[Description("Set up some providers and patients")]
public class SeedCommand : JasperFxAsyncCommand<NetCoreInput>
{
    public override async Task<bool> Execute(NetCoreInput input)
    {
        using var host = input.BuildHost();
        using var scope = host.Services.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();

        await store.Advanced.Clean.DeleteAllDocumentsAsync();
        await store.Advanced.Clean.DeleteAllEventDataAsync();

        // bulk inserts to the store ala await store.BulkInsertAsync(...);

        await using var session = store.LightweightSession();

        // more

        await session.SaveChangesAsync();

        return true;
    }
}
