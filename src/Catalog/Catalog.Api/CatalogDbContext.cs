using Catalog.Api.Brands;
using Catalog.Api.Categories;
using Catalog.Api.Inventories;
using Catalog.Api.Items;
using Catalog.Api.Multimedia;
using Catalog.Api.Prices;
using Catalog.Api.SkuReservations;
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
    public DbSet<Price> Prices { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Media> Medias { get; set; }
    public DbSet<SkuReservation> SkuReservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        const string schema = "catalog";

        // Your typical EF Core mappings
        modelBuilder.Entity<Item>(map =>
        {
            map.ToTable("items", schema);
            map.HasKey(x => x.Id);
            map.Property(x => x.Name).HasMaxLength(200).IsRequired();
            map.Property(x => x.Description).HasMaxLength(1_000).IsRequired(false);
        });

        modelBuilder.Entity<Category>(map =>
        {
            map.ToTable("categories", schema);
            map.HasKey(x => x.Id);
            map.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Brand>(map =>
        {
            map.ToTable("brands", schema);
            map.HasKey(x => x.Id);
            map.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Price>(map =>
        {
            map.ToTable("prices", schema);
            map.HasKey(x => x.Id);
            map.Property(x => x.Value).HasPrecision(18, 2).HasDefaultValue(0m).IsRequired();
        });

        modelBuilder.Entity<Inventory>(map =>
        {
            map.ToTable("inventories", schema);
            map.HasKey(x => x.Id);
            map.Property(x => x.Value).HasDefaultValue(0).IsRequired();
        });

        modelBuilder.Entity<SkuReservation>(map =>
        {
            map.ToTable("sku_reservations", schema);
            map.HasKey(x => x.Unit);
            map.Property(x => x.IsReserved).HasDefaultValue(false).IsRequired();
            map.Property(x => x.ReservedByUsername).HasMaxLength(200).HasDefaultValue(false).IsRequired();
        });

        modelBuilder.Entity<Media>(map =>
        {
            map.ToTable("media", schema);
            map.HasKey(x => x.Id);
            map.Property(x => x.ImageUrl1).HasColumnName("image_url_1").HasMaxLength(255).IsRequired(false);
        });
    }
}
