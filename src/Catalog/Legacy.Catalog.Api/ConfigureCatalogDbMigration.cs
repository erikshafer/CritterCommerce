using Microsoft.EntityFrameworkCore;

namespace Legacy.Catalog.Api;

internal static class ConfigureCatalogDbMigration
{
    public static void ApplyEfDbMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        dataContext.Database.Migrate();
    }
}
