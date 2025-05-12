using JasperFx;
using JasperFx.Resources;
using Marten;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

builder.Services
    .AddMarten(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("marten");
        options.Connection(connectionString!);
        options.DatabaseSchemaName = "inventory";
        options.DisableNpgsqlLogging = true;
    })
    .UseLightweightSessions()
    .IntegrateWithWolverine();

// Do all the necessary database setup on startup
builder.Services.AddResourceSetupOnStartup();

builder.Host.UseWolverine(options =>
{
    options.Policies.AutoApplyTransactions();
    options.Policies.UseDurableLocalQueues();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddWolverineHttp();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapWolverineEndpoints();

app.MapGet("/", ()
    => Results.Redirect("/swagger"));

return await app.RunJasperFxCommands(args);
