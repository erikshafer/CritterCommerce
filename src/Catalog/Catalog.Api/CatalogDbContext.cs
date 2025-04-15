using Microsoft.EntityFrameworkCore;

namespace Catalog.Api;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Your typical EF Core mappings
        modelBuilder.Entity<Item>(map =>
        {
            map.ToTable("items", "catalog");
            map.HasKey(x => x.Id);
            map.Property(x => x.Name).HasMaxLength(200).IsRequired();
            map.Property(x => x.Description).HasMaxLength(1_000).IsRequired(false);
            map.Property(x => x.BrandName).HasMaxLength(200).IsRequired(false);
            map.Property(x => x.CategoryName).HasMaxLength(200).IsRequired(false);
            map.Property(x => x.ImageUrl).HasMaxLength(1_000).IsRequired(false);
            map.Property(x => x.UnitPrice).HasPrecision(18, 2);
            map.Property(x => x.AvailableStock).HasDefaultValue(0).IsRequired();
        });

        modelBuilder.Entity<Category>(map =>
        {
            map.ToTable("brands", "catalog");
            map.HasKey(x => x.Id);
            map.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Brand>(map =>
        {
            map.ToTable("brands", "catalog");
            map.HasKey(x => x.Id);
            map.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });
    }
}
