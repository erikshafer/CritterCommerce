using Microsoft.Data.SqlClient;
using Weasel.SqlServer;
using Weasel.SqlServer.Tables;

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
        var items = new Table(new SqlServerObjectName("catalog", "items"));
        items.AddColumn<Guid>("id").AsPrimaryKey();
        items.AddColumn<string>("name");
        items.AddColumn<string>("description");
        items.AddColumn<string>("brandname");
        items.AddColumn<string>("categoryname");
        items.AddColumn<string>("imageurl");
        items.AddColumn<decimal>("unitprice");
        items.AddColumn<int>("availablestock");

        var connectionString = _configuration.GetConnectionString("sqlserver");
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        await items.ApplyChangesAsync(conn, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
