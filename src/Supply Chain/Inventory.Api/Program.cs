using Inventory;
using JasperFx;
using JasperFx.Core;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using JasperFx.Resources;
using Marten;
using Marten.Events.Projections;
using Marten.Schema;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

builder.Services.AddMarten(opts =>
    {
        var connectionString = builder.Configuration.GetConnectionString("marten");
        opts.Connection(connectionString!);
        opts.DatabaseSchemaName = "inventory";
        opts.DisableNpgsqlLogging = true;

        // A recent optimization
        opts.Projections.UseIdentityMapForAggregates = true;

        // Opts into a mode where Marten is able to rebuild single
        // stream projections faster by building one stream at a time
        // Does require new table migrations for Marten 7 users though
        opts.Events.UseOptimizedProjectionRebuilds = true;

        // The inline projections, with snapshots.
        // With every commit, such as appending an event, updating all associated
        // projections will be batched in a single called to the Postgres database.
        // You however sacrifice some event metadata usage by doing this.
        opts.Projections.Snapshot<InventoryItem>(SnapshotLifecycle.Inline);

        // The async projections with snapshotting.
        // An async daemon will be running in the background, which yes it can be
        // configured and tweaked, and will process all registered projections
        // associated with what has recently been appended to the event store in PostgreSQL.
        // Docs for async daemon: https://martendb.io/events/projections/async-daemon.html#async-projections-daemon
        opts.Projections.Add<InventorySkuProjection>(ProjectionLifecycle.Async);
        opts.Projections.Add<InventoryQuantityOnHandProjection>(ProjectionLifecycle.Async);

        // Before we forget, let's add some indexes to our projections' read models,
        // AKA "Documents", which are made possible by Marten's Document Store functionality.
        opts.Schema.For<InventorySku>()
            .UniqueIndex(UniqueIndexType.Computed, x => x.Sku)
            .Duplicate(x => x.Sku);
        opts.Schema.For<InventoryQuantityOnHand>()
            .Index(x => new { x.Sku, x.QuantityOnHand })
            .Duplicate(x => x.Sku)
            .Duplicate(x => x.QuantityOnHand!);
    })
    // Turn on the async daemon in "Solo" mode
    .AddAsyncDaemon(DaemonMode.Solo)
    // Another performance optimization if you're starting from scratch
    .UseLightweightSessions()
    // This adds configuration with Wolverine's transactional outbox and
    // Marten middleware support to Wolverine
    .IntegrateWithWolverine();

// Do all the necessary database setup on startup
builder.Services.AddResourceSetupOnStartup();

builder.Host.UseWolverine(opts =>
{
    // This is almost an automatic default to have Wolverine apply transactional
    // middleware to any endpoint or handler that uses persistence services
    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableLocalQueues();

    // Retry policies if a Marten concurrency exception is encountered
    opts.OnException<ConcurrencyException>()
        .RetryOnce()
        .Then.RetryWithCooldown(100.Milliseconds(), 250.Milliseconds())
        .Then.Discard();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// To add Wolverine.HTTP services to the IoC container
builder.Services.AddWolverineHttp();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapWolverineEndpoints();

app.MapGet("/", (HttpResponse response) =>
{
    response.Headers.Append("Location", "/swagger");
    response.StatusCode = StatusCodes.Status301MovedPermanently;
}).ExcludeFromDescription();

return await app.RunJasperFxCommands(args);
