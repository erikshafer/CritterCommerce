using Catalog.Api;
using Catalog.Api.Items;
using Catalog.Api.SkuReservations;
using JasperFx;
using JasperFx.Resources;
using Marten;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

// Using Weasel to make sure the catalog-related table exists
builder.Services.AddHostedService<DatabaseSchemaCreator>(); ;

// Adding Marten for persistence
var martenConnectionString = builder.Configuration.GetConnectionString("marten");
builder.Services.AddMarten(opts =>
{
    opts.Connection(martenConnectionString!);
    opts.DatabaseSchemaName = "catalog";
})
// Optionally add Marten/Postgresql integration with Wolverine's outbox
.IntegrateWithWolverine();

builder.Services.AddResourceSetupOnStartup();

var postgresConnectionString = builder.Configuration.GetConnectionString("postgres");
builder.Services.AddDbContextWithWolverineIntegration<CatalogDbContext>(opts =>
    opts
        .UseNpgsql(postgresConnectionString)
        .UseSnakeCaseNamingConvention(),
    "catalog");

// builder.Services.AddDbContextWithWolverineIntegration<CatalogDbContext>(opts =>
// {
//     var connectionString = builder.Configuration.GetConnectionString("postgres");
//     opts.UseNpgsql(connectionString!)
//         .UseSnakeCaseNamingConvention();
// },"catalog");

builder.Host.UseWolverine(opts =>
{
    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableLocalQueues();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapPost("/api/sku-reservations", (ReserveSku cmd, IMessageBus bus) =>
    bus.InvokeAsync<SkuReserved>(cmd));

app.MapPost("/api/sku-reservations/unreserve", (UnreserveSku cmd, IMessageBus bus) =>
    bus.InvokeAsync<SkuUnreserved>(cmd));

app.MapPost("/api/sku-item-assignments", (AssignSkuToDraftItem cmd, IMessageBus bus) =>
    bus.InvokeAsync<AssignedSkuToDraftItem>(cmd));

return await app.RunJasperFxCommands(args);
