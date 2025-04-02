using Microsoft.EntityFrameworkCore;

namespace Catalog.Api;

public class ItemsDbContext : DbContext
{
    public ItemsDbContext(DbContextOptions<ItemsDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Your typical EF Core mapping
        modelBuilder.Entity<Item>(map =>
        {
            map.ToTable("items", "sample");
            map.HasKey(x => x.Id);
            map.Property(x => x.Name);
        });
    }
}
