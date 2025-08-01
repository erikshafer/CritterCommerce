using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Legacy.Catalog.Application;

public static class ConfigureCatalogDbMigration
{
    public static void ApplyEfDbMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        dataContext.Database.Migrate();
    }
}
