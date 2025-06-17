using Catalog.Api;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

var postgresConnectionString = builder.Configuration.GetConnectionString("postgres");

builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseNpgsql(postgresConnectionString);
    options.UseSnakeCaseNamingConvention();
});

builder.Host.UseWolverine();

builder.Services.AddWolverineHttp();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapWolverineEndpoints();

app.MapGet("/", (HttpResponse response) =>
{
    response.Headers.Append("Location", "/swagger");
    response.StatusCode = StatusCodes.Status301MovedPermanently;
}).ExcludeFromDescription();

app.Run();
