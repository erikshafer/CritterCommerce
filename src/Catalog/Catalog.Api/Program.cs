using Marten;
using Oakton;
using Oakton.Resources;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyOaktonExtensions();

// Adding Marten for persistence
builder.Services.AddMarten(opts =>
    {
        var connectionString = builder
            .Configuration
            .GetConnectionString("postgres");
        opts.Connection(connectionString!);
        opts.DatabaseSchemaName = "catalog";
    })
    // Optionally add Marten/Postgresql integration with Wolverine's outbox
    .IntegrateWithWolverine();

builder.Services.AddResourceSetupOnStartup();

// Wolverine usage is required for WolverineFx.Http
builder.Host.UseWolverine(opts =>
{
    // This middleware will apply to the HTTP endpoints as well
    opts.Policies.AutoApplyTransactions();

    // Setting up the outbox on all locally handled background tasks
    opts.Policies.UseDurableLocalQueues();

    // NOT USING
    // I've added persistent inbox behavior to the "important" local queue
    // opts.LocalQueue("important")
    //     .UseDurableInbox();
});

builder.Services.AddOpenApi(); // TODO: learn what this all entails, it's new to me

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWolverineHttp();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection(); // TODO: Need?

// Let's add in Wolverine HTTP endpoints to the routing tree
app.MapWolverineEndpoints();

// A lot of Wolverine and Marten diagnostics and administrative tools
// come through Oakton command line support
return await app.RunOaktonCommands(args);
