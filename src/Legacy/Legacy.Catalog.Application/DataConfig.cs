using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Legacy.Catalog.Application;

public static class DataConfig
{
    private const string SchemaName = "legacy_catalog";
    private const string ConnectionStringKey = "SqlServer";

    public static IServiceCollection AddLegacyCatalogData(this IServiceCollection services, IConfiguration config) =>
        services.AddEntityFramework(config);

    private static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration config) =>
        services
            .AddDbContext<CatalogDbContext>(options =>
            {
                var connectionString = config.GetConnectionString(ConnectionStringKey);

                options.UseSqlServer(
                    connectionString,
                    builder => builder.MigrationsHistoryTable("__EFCoreMigrationsHistory", SchemaName));
            });
}
