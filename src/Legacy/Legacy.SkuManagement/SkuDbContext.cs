using Microsoft.EntityFrameworkCore;

namespace Legacy.SkuManagement;

public class SkuDbContext : DbContext
{
    public SkuDbContext(DbContextOptions<SkuDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("legacy_sku_management");
    }
}
