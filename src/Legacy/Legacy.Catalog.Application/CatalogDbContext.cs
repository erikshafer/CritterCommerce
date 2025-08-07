using Legacy.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Legacy.Catalog.Application;

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

    private const string SchemaName = "legacy_catalog";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        // Your typical EF Core mappings
        modelBuilder.Entity<Item>(map =>
        {
            map.ToTable("items");
            map.HasKey(x => x.Id);
            map.Property(x => x.Id).ValueGeneratedNever(); // Disable IDENTITY_INSERT
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
            map.Property(x => x.Quantity).HasDefaultValue(0).IsRequired();
        });

        modelBuilder.Entity<Media>(map =>
        {
            map.ToTable("media");
            map.HasKey(x => x.Id);
            map.Property(x => x.ImageUrl1).HasColumnName("image_url_1").HasMaxLength(512).IsRequired(false);
            map.Property(x => x.ImageUrl1).HasColumnName("image_url_2").HasMaxLength(512).IsRequired(false);
            map.Property(x => x.ImageUrl1).HasColumnName("image_url_3").HasMaxLength(512).IsRequired(false);
        });
    }
}
