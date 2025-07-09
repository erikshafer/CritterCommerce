using Microsoft.EntityFrameworkCore;

namespace Legacy.SkuManagement.Api;

internal static class ConfigureSkuDbMigration
{
    public static void ApplyEfDbMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<SkuDbContext>();
        dataContext.Database.Migrate();
    }
}
