using Microsoft.EntityFrameworkCore;

namespace Catalog.Api;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Your typical EF Core mapping
        modelBuilder.Entity<Item>(map =>
        {
            map.ToTable("items", "catalog");
            map.HasKey(x => x.Id);
            map.Property(x => x.Name);
            map.Property(x => x.Description).IsRequired(false);
            map.Property(x => x.BrandName).IsRequired(false);
            map.Property(x => x.CategoryName).IsRequired(false);
            map.Property(x => x.ImageUrl).IsRequired(false);
            map.Property(x => x.UnitPrice).HasPrecision(18, 2);
            map.Property(x => x.AvailableStock).HasDefaultValue(0);
        });
    }
}
