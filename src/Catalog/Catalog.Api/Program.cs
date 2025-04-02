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
builder.Services.AddHostedService<DatabaseSchemaCreator>();

// Adding Marten for persistence
builder.Services.AddMarten(opts =>
    {
        var connectionString = builder.Configuration.GetConnectionString("postgres");
        opts.Connection(connectionString!);
        opts.DatabaseSchemaName = "catalog";
    })
    // Optionally add Marten/Postgresql integration with Wolverine's outbox
    .IntegrateWithWolverine();

builder.Services.AddResourceSetupOnStartup();

// If you're okay with this, this will register the DbContext as normally,
// but make some Wolverine specific optimizations at the same time
builder.Services.AddDbContextWithWolverineIntegration<CatalogDbContext>(opts =>
{
    var connectionString = builder.Configuration.GetConnectionString("sqlserver");
    opts.UseSqlServer(connectionString);
},"wolverine");

// Wolverine usage is required for WolverineFx.Http
builder.Host.UseWolverine(opts =>
{
    // This middleware will apply to the HTTP endpoints as well
    opts.Policies.AutoApplyTransactions();

    // Setting up the outbox on all locally handled background tasks
    opts.Policies.UseDurableLocalQueues();
});

builder.Services.AddOpenApi(); // TODO: learn what this all entails, it's new to me

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWolverineHttp();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Let's add in Wolverine HTTP endpoints to the routing tree
app.MapWolverineEndpoints();

// A lot of Wolverine and Marten diagnostics and administrative tools
// come through Oakton command line support
return await app.RunOaktonCommands(args);
