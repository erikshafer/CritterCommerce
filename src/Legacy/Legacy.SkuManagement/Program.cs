using Legacy.SkuManagement;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var postgresConnectionString = builder.Configuration.GetConnectionString("postgres");

builder.Services.AddDbContext<SkuDbContext>(options =>
{
    options.UseNpgsql(
        postgresConnectionString,
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
