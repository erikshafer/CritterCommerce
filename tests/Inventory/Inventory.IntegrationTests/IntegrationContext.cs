using Alba;
using Marten;
using Marten.Storage;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Runtime;

namespace Inventory.IntegrationTests;

[Collection("integration")]
public abstract class IntegrationContext : IAsyncLifetime
{
    private readonly AppFixture _fixture;

    protected IntegrationContext(AppFixture fixture)
    {
        _fixture = fixture;
        Runtime = (WolverineRuntime)fixture.Host!.Services.GetRequiredService<IWolverineRuntime>();
    }

    public WolverineRuntime Runtime { get; }

    public IAlbaHost Host => _fixture.Host!;
    public DocumentStore Store => (DocumentStore)_fixture.Host!.Services.GetRequiredService<IDocumentStore>();

    public virtual async Task InitializeAsync() => await ResetAllDataAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task ResetAllDataAsync(CancellationToken cancellation = default)
    {
        foreach (IMartenDatabase database in (await Store.Tenancy.BuildDatabases()).OfType<IMartenDatabase>())
            await database.DeleteAllDocumentsAsync(cancellation);
    }
}
