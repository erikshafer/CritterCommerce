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

builder.Services.AddMarten(opts =>
    {
        var connectionString = builder.Configuration.GetConnectionString("marten");
        opts.Connection(connectionString!);
        opts.DatabaseSchemaName = "inventory";
        opts.DisableNpgsqlLogging = true;

        // A recent optimization
        opts.Projections.UseIdentityMapForAggregates = true;

        opts.Projections.AggregatorFor<Inventory.Inventory>();
    })
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
builder.Services.AddOpenApi();
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
