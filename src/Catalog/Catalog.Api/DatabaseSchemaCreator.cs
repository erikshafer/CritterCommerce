using Npgsql;
using Weasel.Postgresql;
using Weasel.Postgresql.Tables;

namespace Catalog.Api;

public class DatabaseSchemaCreator : IHostedService
{
    private readonly IConfiguration _configuration;

    public DatabaseSchemaCreator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var items = new Table(new PostgresqlObjectName("catalog", "items"));
        items.AddColumn<Guid>("id").AsPrimaryKey();
        items.AddColumn<string>("name");
        items.AddColumn<string>("description");
        items.AddColumn<string>("brand_name");
        items.AddColumn<string>("category_name");
        items.AddColumn<string>("image_url");
        items.AddColumn<decimal>("unit_price");
        items.AddColumn<int>("available_stock").DefaultValue(0);

        var connectionString = _configuration.GetConnectionString("postgres");
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        await items.ApplyChangesAsync(conn, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
