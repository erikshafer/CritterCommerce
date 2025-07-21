using Alba;
using Marten;
using Oakton;
using Testcontainers.PostgreSql;
using Wolverine;

namespace Inventory.IntegrationTests;

public class AppFixture : IAsyncLifetime
{
    public IAlbaHost? Host { get; private set; }

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        var conn = _postgreSqlContainer.GetConnectionString();

        OaktonEnvironment.AutoStartHost = true;

        Host = await AlbaHost.For<Program>(x =>
        {
            x.ConfigureServices(services =>
            {
                services.ConfigureMarten(opts =>
                {
                    opts.Connection(conn);
                });

                services.DisableAllExternalWolverineTransports();
            });

        });
    }

    public async Task DisposeAsync()
    {
        await Host!.StopAsync();
        Host.Dispose();

        await _postgreSqlContainer.StopAsync();
    }
}
