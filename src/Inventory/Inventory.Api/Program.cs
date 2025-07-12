using System.Text.Json.Serialization;
using Inventory.Api;
using Inventory.Api.Inbound;
using Inventory.Api.Inbound.Views;
using Inventory.Api.Locations;
using Inventory.Api.Procurement;
using Inventory.Api.Receiving;
using Inventory.Api.Receiving.Views;
using Inventory.Api.Vendors;
using Inventory.Api.WarehouseLevels;
using Inventory.Api.WarehouseLevels.Lots;
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
using Wolverine.Http.FluentValidation;
using Wolverine.Marten;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

var martenConnectionString = builder.Configuration.GetConnectionString("Marten")
                             ?? throw new Exception("Marten connection string not found");

builder.Services.AddMarten(opts =>
    {
        opts.Connection(martenConnectionString);
        opts.AutoCreateSchemaObjects = AutoCreate.All; // Dev mode: create tables if missing
        opts.UseSystemTextJsonForSerialization(); // Opt-in, recommended for new projects

        opts.DatabaseSchemaName = Constants.Inventory;
        opts.DisableNpgsqlLogging = true;

        // The inline projections, with snapshots.
        // With every commit, such as appending an event, updating all associated
        // projections will be batched in a single call to the Postgres database.
        // However, you sacrifice some event metadata usage by doing this.
        opts.Projections
            .Snapshot<FreightShipment>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Origin)
            .Duplicate(x => x.Destination);

        opts.Projections
            .Snapshot<ReceivedShipment>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id);

        opts.Projections
            .Snapshot<InventoryLevel>(SnapshotLifecycle.Inline)
            .Identity(x => x.Id)
            .Duplicate(x => x.Sku);

        // The async projections with snapshotting.
        // An async daemon will be running in the background, which yes it can be
        // configured and tweaked, and will process all registered projections
        // associated with what has recently been appended to the event store in PostgreSQL.
        // Docs for async daemon: https://martendb.io/events/projections/async-daemon.html#async-projections-daemon
        opts.Projections.Add<ExpectedQuantityAnticipatedProjection>(ProjectionLifecycle.Async);
        opts.Projections.Add<DailyShipmentsProjection>(ProjectionLifecycle.Async);
        opts.Projections.Add<FreightShipmentProjection>(ProjectionLifecycle.Async);
        opts.Projections.Add<WarehouseLotsProjection>(ProjectionLifecycle.Async);

        opts.RegisterDocumentType<Location>();
        opts.Schema.For<Location>()
            .Identity(x => x.Id)
            .Duplicate(x => x.Name);

        opts.RegisterDocumentType<Vendor>();
        opts.Schema.For<Vendor>()
            .Identity(x => x.Id)
            .Duplicate(x => x.Name);

        opts.RegisterDocumentType<ProcurementOrder>();
        opts.Schema.For<ProcurementOrder>()
            .Identity(x => x.Id)
            .Duplicate(x => x.VendorId) // Consider making this a foreign key to the Vendor docs
            .Duplicate(x => x.TrackingNumber); // Could add the entire document's properties here, but

        opts.Schema.For<WarehouseLotsView>()
            .Duplicate(x => x.Warehouse)
            .Duplicate(x => x.Lot);
    })
    .InitializeWith(new InitialData(
        InitialData.ConcatDataSets(
            LocationsDatasets.Data,
            VendorsDatasets.Data,
            ProcurementOrdersDatasets.Data)))
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

builder.Services.ConfigureSystemTextJsonForWolverineOrMinimalApi(opts =>
{
    opts.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Host.UseWolverine(opts =>
{
    // This is almost an automatic default to have Wolverine apply transactional
    // middleware to any endpoint or handler that uses persistence services
    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableLocalQueues();
    // Opt into the transactional inbox/outbox on all messaging endpoints
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();

    // Retry policies if a Marten concurrency exception is encountered
    opts.OnException<ConcurrencyException>()
        .RetryOnce()
        .Then.RetryWithCooldown(100.Milliseconds(), 250.Milliseconds())
        .Then.Discard();

    opts.UseFluentValidation();

    opts.UseRabbitMq(new Uri("amqp://localhost"))
        .AutoProvision()
        .UseConventionalRouting();

    opts.PublishAllMessages()
        .ToRabbitExchange(Constants.Inventory)
        .UseDurableOutbox();
});

builder.Services.AddSingleton<IFacilityLotService, FacilityLotService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWolverineHttp();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapWolverineEndpoints(opts =>
{
    opts.UseFluentValidationProblemDetailMiddleware();
});

app.MapGet("/", (HttpResponse response) =>
{
    response.Headers.Append("Location", "/swagger");
    response.StatusCode = StatusCodes.Status301MovedPermanently;
}).ExcludeFromDescription();

return await app.RunJasperFxCommands(args);
