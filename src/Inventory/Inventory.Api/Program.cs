using System.Text.Json.Serialization;
using Inventory.Api;
using Inventory.Api.Inbound;
using Inventory.Api.Inbound.Projections;
using Inventory.Api.Locations;
using Inventory.Api.Procurement;
using Inventory.Api.Receiving.Projections;
using Inventory.Api.Vendors;
using Inventory.Api.WarehouseInventories;
using JasperFx;
using JasperFx.Core;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using JasperFx.Resources;
using Marten;
using Marten.Events.Projections;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Marten;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Wolverine.Transports;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

var martenConnectionString = builder.Configuration.GetConnectionString("marten")
                             ?? throw new Exception("Marten connection string not found");

builder.Services.AddMarten(opts =>
    {
        opts.Connection(martenConnectionString);
        opts.AutoCreateSchemaObjects = AutoCreate.All; // Dev mode: create tables if missing
        opts.UseSystemTextJsonForSerialization(); // Opt-in, recommended for new projects

        opts.DatabaseSchemaName = "inventory";
        opts.DisableNpgsqlLogging = true;

        // The inline projections, with snapshots.
        // With every commit, such as appending an event, updating all associated
        // projections will be batched in a single call to the Postgres database.
        // However, you sacrifice some event metadata usage by doing this.
        opts.Projections
            .Snapshot<InventoryItem>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Sku);

        opts.Projections
            .Snapshot<FreightShipment>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Origin)
            .Duplicate(x => x.Destination);

        // The async projections with snapshotting.
        // An async daemon will be running in the background, which yes it can be
        // configured and tweaked, and will process all registered projections
        // associated with what has recently been appended to the event store in PostgreSQL.
        // Docs for async daemon: https://martendb.io/events/projections/async-daemon.html#async-projections-daemon
        opts.Projections.Add<ExpectedQuantityAnticipatedProjection>(ProjectionLifecycle.Async);
        opts.Projections.Add<DailyShipmentsProjection>(ProjectionLifecycle.Async);

        opts.RegisterDocumentType<Location>();
        opts.Schema.For<Location>()
            .Identity(x => x.Id)
            .Duplicate(x => x.Name);

        opts.RegisterDocumentType<Vendor>();
        opts.Schema.For<Vendor>()
            .Identity(x => x.Id)
            .Duplicate(x => x.Name);

        opts.RegisterDocumentType<ReceivedProcurementOrder>();
        opts.Schema.For<ReceivedProcurementOrder>()
            .Identity(x => x.Id)
            .Duplicate(x => x.VendorId) // Consider making this a foreign key to the Vendor docs
            .Duplicate(x => x.TrackingNumber); // Could add the entire document's properties here, but
    })
    .InitializeWith(new InventoryInitialData(
        InventoryInitialData.ConcatDataSets(
            LocationsDatasets.Data,
            VendorsDatasets.Data,
            ReceivedProcurementOrdersDatasets.Data)))
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

    opts.UseFluentValidation();

    // The Rabbit MQ transport supports all three types of listeners
    opts.UseRabbitMq()
        // Directs Wolverine to build any declared queues, exchanges, or
        // bindings with the Rabbit MQ broker as part of bootstrapping time
        .AutoProvision();

    opts.PersistMessagesWithPostgresql(martenConnectionString, "listeners"); // The durable mode requires some sort of envelope storage

    opts.ListenToRabbitQueue("inline")
        .ProcessInline() // Process inline, default is with one listener
        .ListenerCount(5); // But, you can use multiple, parallel listeners

    opts.ListenToRabbitQueue("buffered")
        .BufferedInMemory(new BufferingLimits(1000, 500)); // Buffer the messages in memory for increased throughput

    opts.ListenToRabbitQueue("durable")
        .UseDurableInbox(new BufferingLimits(1000, 500)); // Opt into durable inbox mechanics

    opts.ListenToRabbitQueue("ordered")
        // This option is available on all types of Wolverine
        // endpoints that can be configured to be a listener
        .ListenWithStrictOrdering();
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
