using Catalog;
using Catalog.DraftProducts;
using Catalog.DraftTaxonomy;
using Catalog.Products;
using JasperFx;
using JasperFx.Core;
using Marten;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

var martenConnectionString = builder.Configuration.GetConnectionString("Marten")
                             ?? throw new Exception("Marten connection string not found");

builder.Services.AddMarten(opts =>
{
    opts.Connection(martenConnectionString);
    opts.AutoCreateSchemaObjects = AutoCreate.All; // Dev mode: create tables if missing
    opts.UseSystemTextJsonForSerialization(); // Opt-in, recommended for new projects

    opts.DatabaseSchemaName = Constants.Catalog;
    opts.DisableNpgsqlLogging = true;

    // Add projections
    opts.AddDraftProductProjections();
    opts.AddProductProjections();
    opts.AddDraftCategoryProjections();
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

    // TODO: FluentValidation, RabbitMQ, PublishAllMessages, etc, or figure something else out =)
});


// Add services (domain, etc)
builder.Services.AddDraftProductServices();
builder.Services.AddProductServices();
builder.Services.AddDraftCategoryServices();

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
    // opts.UseFluentValidationProblemDetailMiddleware(); // TODO FluentValidation
});

app.MapGet("/", (HttpResponse response) =>
{
    response.Headers.Append("Location", "/swagger");
    response.StatusCode = StatusCodes.Status301MovedPermanently;
}).ExcludeFromDescription();

return await app.RunJasperFxCommands(args);

public partial class Program { }
