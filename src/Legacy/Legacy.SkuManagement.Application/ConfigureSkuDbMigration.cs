using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Legacy.SkuManagement;

public static class ConfigureSkuDbMigration
{
    public static void ApplyEfDbMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<SkuDbContext>();
        dataContext.Database.Migrate();
    }
}
