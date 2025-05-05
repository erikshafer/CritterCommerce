using Catalog.Api.Items;
using JasperFx.CommandLine;
using JasperFx.Core;
using Spectre.Console;

namespace Catalog.Api;

[Description("Set up some items")]
public class SeedCommand : JasperFxAsyncCommand<NetCoreInput>
{
    public override async Task<bool> Execute(NetCoreInput input)
    {
        using var host = input.BuildHost();
        using var scope = host.Services.CreateScope();

        var db = scope.ServiceProvider.GetService<CatalogDbContext>();
        await db.Database.EnsureCreatedAsync();

        var combId1 = CombGuidIdGeneration.NewGuid();
        var item1 = new Item { Id = combId1, Name = "Bouncy House", Description = string.Empty };

        var combId2 = CombGuidIdGeneration.NewGuid();
        var item2 = new Item { Id = combId2, Name = "Portable Battery", Description = string.Empty };

        db.Items.AddRange(item1, item2);

        await db.SaveChangesAsync();

        AnsiConsole.Markup("[green]All data loaded successfully![/]");

        return true;
    }
}
