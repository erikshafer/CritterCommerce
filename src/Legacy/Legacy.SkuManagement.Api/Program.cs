using Legacy.SkuManagement.Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SkuDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        x => x.MigrationsHistoryTable("__EFMigrationsHistory", "legacy_sku_management"));
    options.UseSnakeCaseNamingConvention();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.ApplyEfDbMigration();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", (HttpResponse response) =>
{
    response.Headers.Append("Location", "/swagger");
    response.StatusCode = StatusCodes.Status301MovedPermanently;
}).ExcludeFromDescription();

app.Run();
