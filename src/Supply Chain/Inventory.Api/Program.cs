using JasperFx;
using JasperFx.Resources;
using Marten;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

// Adding Marten for persistence
builder.Services
    .AddMarten(opts =>
    {
        var connectionString = builder.Configuration.GetConnectionString("marten");
        opts.Connection(connectionString!);
        opts.DatabaseSchemaName = "inventory";
    })
    // Optionally add Marten/Postgresql integration with Wolverine's outbox
    .IntegrateWithWolverine();

builder.Services.AddResourceSetupOnStartup();

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

return await app.RunJasperFxCommands(args);
