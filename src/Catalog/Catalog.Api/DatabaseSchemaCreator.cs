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
        const string schema = "catalog";

        var items = new Table(new PostgresqlObjectName(schema, "items"));
        items.AddColumn<Guid>("id").AsPrimaryKey();
        items.AddColumn<string>("name");
        items.AddColumn<string>("description");

        var categories = new Table(new PostgresqlObjectName(schema, "categories"));
        categories.AddColumn<Guid>("id").AsPrimaryKey();
        categories.AddColumn<string>("name");

        var brands = new Table(new PostgresqlObjectName(schema, "brands"));
        brands.AddColumn<Guid>("id").AsPrimaryKey();
        brands.AddColumn<string>("name");

        var prices = new Table(new PostgresqlObjectName(schema, "prices"));
        prices.AddColumn<Guid>("id").AsPrimaryKey();
        prices.AddColumn<string>("value");

        var inventories = new Table(new PostgresqlObjectName(schema, "inventories"));
        inventories.AddColumn<Guid>("id").AsPrimaryKey();
        inventories.AddColumn<string>("value");

        var skuReservations = new Table(new PostgresqlObjectName(schema, "sku_reservations"));
        skuReservations.AddColumn<int>("unit").AsPrimaryKey();
        skuReservations.AddColumn<bool>("is_reserved");
        skuReservations.AddColumn<string>("reserved_by_username");

        var media = new Table(new PostgresqlObjectName(schema, "media"));
        media.AddColumn<Guid>("id").AsPrimaryKey();
        media.AddColumn<string>("image_url_1");

        var skuItemAssignments = new Table(new PostgresqlObjectName(schema, "sku_item_assignments"));
        skuItemAssignments.AddColumn<int>("sku").AsPrimaryKey();
        skuItemAssignments.AddColumn<Guid>("item_id");

        var connectionString = _configuration.GetConnectionString("postgres");
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        await items.ApplyChangesAsync(conn, cancellationToken);
        await categories.ApplyChangesAsync(conn, cancellationToken);
        await brands.ApplyChangesAsync(conn, cancellationToken);
        await prices.ApplyChangesAsync(conn, cancellationToken);
        await inventories.ApplyChangesAsync(conn, cancellationToken);
        await skuReservations.ApplyChangesAsync(conn, cancellationToken);
        await media.ApplyChangesAsync(conn, cancellationToken);
        await skuItemAssignments.ApplyChangesAsync(conn, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
