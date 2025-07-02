using JasperFx;
using Marten;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyJasperFxExtensions();

builder.Services.AddMarten(options =>
{
    var martenConnectionString = builder.Configuration.GetConnectionString("marten")
                                 ?? throw new Exception("Marten connection string not found");
    options.Connection(martenConnectionString);
    options.UseSystemTextJsonForSerialization();
    options.DatabaseSchemaName = "catalog";
});

builder.Host.UseWolverine();

builder.Services.AddWolverineHttp();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", (HttpResponse response) =>
{
    response.Headers.Append("Location", "/swagger");
    response.StatusCode = StatusCodes.Status301MovedPermanently;
}).ExcludeFromDescription();

return await app.RunJasperFxCommands(args);
