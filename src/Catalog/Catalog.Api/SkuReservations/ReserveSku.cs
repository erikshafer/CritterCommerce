using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.SkuReservations;

public sealed record ReserveSku(int Unit, string Username);

public sealed record SkuReserved(int Unit, string ReservedByUsername);

public static class ReserveSkuHandler
{
    public static async Task<SkuReserved> Handle(ReserveSku command, CatalogDbContext db)
    {
        var existingSku = await db.SkuReservations.FirstOrDefaultAsync(x => x.Unit == command.Unit);

        if (existingSku?.IsReserved is true)
            throw new Exception($"SKU '{existingSku.Unit}' already reserved");

        var sku = new SkuReservation { Unit = command.Unit, IsReserved = true, ReservedByUsername = command.Username };

        db.SkuReservations.Add(sku);

        return new SkuReserved(sku.Unit, command.Username);
    }
}
