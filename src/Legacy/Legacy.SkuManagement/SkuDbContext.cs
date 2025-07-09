using Legacy.SkuManagement.Items;
using Legacy.SkuManagement.SkuReservations;
using Microsoft.EntityFrameworkCore;

namespace Legacy.SkuManagement;

public class SkuDbContext : DbContext
{
    public SkuDbContext(DbContextOptions<SkuDbContext> options) : base(options)
    {
    }

    public DbSet<SkuReservation> SkuReservations { get; set; } = null!;
    public DbSet<SkuItemAssignment> SkuItemAssignments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("legacy_sku_management");

        modelBuilder.Entity<SkuReservation>(map =>
        {
            map.ToTable("sku_reservations");
            map.HasKey(x => x.Unit);
            map.Property(x => x.IsReserved).HasDefaultValue(false).IsRequired();
            map.Property(x => x.ReservedByUsername).HasMaxLength(200).HasDefaultValue(false).IsRequired();
        });

        modelBuilder.Entity<SkuItemAssignment>(map =>
        {
            map.ToTable("sku_item_assignments");
            map.HasKey(x => x.Sku);
            map.Property(x => x.ItemId).HasMaxLength(128).IsRequired();
        });
    }
}
