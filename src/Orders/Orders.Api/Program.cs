using System.Text.Json.Serialization;
using JasperFx;
using JasperFx.Core;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using JasperFx.Resources;
using Marten;
using Orders.Api;
using Orders.Api.Orders.Views;
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

        opts.DatabaseSchemaName = Constants.Orders;
        opts.DisableNpgsqlLogging = true;

        // Projections
        opts.Projections.Add<OrderFulfillmentDashboardProjection>(ProjectionLifecycle.Async);
        opts.Projections.Add<CustomerOrderHistoryProjection>(ProjectionLifecycle.Async);

        // Documents
        opts.Schema.For<OrderFulfillmentDashboardView>()
            .Duplicate(x => x.Placed)
            .Duplicate(x => x.Confirmed)
            .Duplicate(x => x.Confirmed)
            .Duplicate(x => x.Fulfilled);
        opts.Schema.For<CustomerOrderHistoryView>()
            .GinIndexJsonData(); // Optimizes querying nested JSONB structures in Postgres

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
        .ToRabbitExchange(Constants.Orders)
        .UseDurableOutbox();
});

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

public partial class Program { }
