using Legacy.Catalog.Api.Brands;
using Legacy.Catalog.Api.Categories;
using Legacy.Catalog.Api.Inventories;
using Legacy.Catalog.Api.Items;
using Legacy.Catalog.Api.Multimedia;
using Legacy.Catalog.Api.Prices;
using Microsoft.EntityFrameworkCore;

namespace Legacy.Catalog.Api;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Price> Prices { get; set; } = null!;
    public DbSet<Inventory> Inventories { get; set; } = null!;
    public DbSet<Media> Medias { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("legacy_catalog");

        // Your typical EF Core mappings
        modelBuilder.Entity<Item>(map =>
        {
            map.ToTable("items");
            map.HasKey(x => x.Id);
            map.Property(x => x.Name).HasMaxLength(200).IsRequired();
            map.Property(x => x.Description).HasMaxLength(1_000).IsRequired(false);
        });

        modelBuilder.Entity<Category>(map =>
        {
            map.ToTable("categories");
            map.HasKey(x => x.Id);
            map.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Brand>(map =>
        {
            map.ToTable("brands");
            map.HasKey(x => x.Id);
            map.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Price>(map =>
        {
            map.ToTable("prices");
            map.HasKey(x => x.Id);
            map.Property(x => x.Value).HasPrecision(18, 2).HasDefaultValue(0m).IsRequired();
        });

        modelBuilder.Entity<Inventory>(map =>
        {
            map.ToTable("inventories");
            map.HasKey(x => x.Id);
            map.Property(x => x.Value).HasDefaultValue(0).IsRequired();
        });

        modelBuilder.Entity<Media>(map =>
        {
            map.ToTable("media");
            map.HasKey(x => x.Id);
            map.Property(x => x.ImageUrl1).HasColumnName("image_url_1").HasMaxLength(255).IsRequired(false);
        });
    }
}
