using System.Text.Json.Serialization;
using Inventory.Api.Documents;
using Inventory.Api.Inbound.Projections;
using Inventory.Api.Inventories;
using Inventory.Api.Receiving.Projections;
using Inventory.Api.Locations;
using JasperFx;
using JasperFx.Core;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using JasperFx.Resources;
using Marten;
using Marten.Events.Projections;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

builder.Services.AddMarten(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("marten")
                               ?? throw new Exception("Marten connection string not found");
        options.Connection(connectionString);
        options.AutoCreateSchemaObjects = AutoCreate.All; // Dev mode: create tables if missing
        options.DatabaseSchemaName = "inventory";
        options.DisableNpgsqlLogging = true;
        options.Projections.UseIdentityMapForAggregates = true; // A recent optimization

        // Opts into a mode where Marten is able to rebuild single
        // stream projections faster by building one stream at a time
        // Does require new table migrations for Marten 7 users though
        options.Events.UseOptimizedProjectionRebuilds = true;

        // The inline projections, with snapshots.
        // With every commit, such as appending an event, updating all associated
        // projections will be batched in a single call to the Postgres database.
        // However, you sacrifice some event metadata usage by doing this.
        options.Projections
            .Snapshot<InventoryItem>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Sku);

        // Inline projection, no snapshots.
        // Note that this is different from aggregating a domain model like
        // FreightShipment using AggregateStreamAsync<T>. Instead, we're producing a
        // derived view (or read model) designed for fast queries, based on a subset of event data.
        options.Projections.Add<ShipmentViewProjection>(ProjectionLifecycle.Inline);

        // The async projections with snapshotting.
        // An async daemon will be running in the background, which yes it can be
        // configured and tweaked, and will process all registered projections
        // associated with what has recently been appended to the event store in PostgreSQL.
        // Docs for async daemon: https://martendb.io/events/projections/async-daemon.html#async-projections-daemon
        options.Projections.Add<ExpectedQuantityAnticipatedProjection>(ProjectionLifecycle.Async);
        options.Projections.Add<DailyShipmentsProjection>(ProjectionLifecycle.Async);

        options.RegisterDocumentType<Location>();
        options.Schema.For<Location>()
            .Identity(x => x.Id)
            .Duplicate(x => x.Name);

        options.RegisterDocumentType<Vendor>();
        options.Schema.For<Vendor>()
            .Identity(x => x.Id)
            .Duplicate(x => x.Name);

        options.RegisterDocumentType<ProcurementOrder>();
        options.Schema.For<ProcurementOrder>()
            .Identity(x => x.Id)
            .ForeignKey<Vendor>(x => x.VendorId);
    })
    // Turn on the async daemon in "Solo" mode
    .AddAsyncDaemon(DaemonMode.Solo)
    // Another performance optimization if you're starting from scratch
    .UseLightweightSessions()
    // This adds configuration with Wolverine's transactional outbox and
    // Marten middleware support to Wolverine
    .IntegrateWithWolverine(config =>
    {
        config.UseWolverineManagedEventSubscriptionDistribution = true;
    });

// Do all the necessary database setup on startup
builder.Services.AddResourceSetupOnStartup();

builder.Services.ConfigureSystemTextJsonForWolverineOrMinimalApi(o =>
{
    o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

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
