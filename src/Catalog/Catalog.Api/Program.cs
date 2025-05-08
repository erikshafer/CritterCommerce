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

// Using Weasel to make sure the items table exists
builder.Services.AddHostedService<DatabaseSchemaCreator>(); ;

// Adding Marten for persistence
builder.Services.AddMarten(opts =>
{
    var connectionString = builder.Configuration.GetConnectionString("marten");
    opts.Connection(connectionString!);
    opts.DatabaseSchemaName = "catalog";
})
// Optionally add Marten/Postgresql integration with Wolverine's outbox
.IntegrateWithWolverine();

builder.Services.AddResourceSetupOnStartup();

builder.Services.AddDbContextWithWolverineIntegration<CatalogDbContext>(opts =>
{
    var connectionString = builder.Configuration.GetConnectionString("postgres");
    opts.UseNpgsql(connectionString!)
        .UseSnakeCaseNamingConvention();
},"catalog");

builder.Host.UseWolverine(opts =>
{
    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableLocalQueues();
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWolverineHttp();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapWolverineEndpoints();

app.MapGet("/", ()
    => Results.Redirect("/swagger"));

app.MapPost("/api/sku-reservations", (ReserveSku cmd, IMessageBus bus) =>
    bus.InvokeAsync<SkuReserved>(cmd));

app.MapPost("/api/sku-reservations/unreserve", (UnreserveSku cmd, IMessageBus bus) =>
    bus.InvokeAsync<SkuUnreserved>(cmd));

app.MapPost("/api/assign-sku-to-item", (AssignSkuToDraftItem cmd, IMessageBus bus) =>
    bus.InvokeAsync<AssignedSkuToDraftItem>(cmd));

return await app.RunJasperFxCommands(args);
