using Catalog.Api;
using Marten;
using Microsoft.EntityFrameworkCore;
using Oakton;
using Oakton.Resources;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyOaktonExtensions();

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

app.MapGet("/", () => Results.Redirect("/swagger"));

return await app.RunOaktonCommands(args);
