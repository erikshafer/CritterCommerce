using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.SkuReservations;

public sealed record UnreserveSku(int Unit, string Username);

public sealed record SkuUnreserved(int Unit, string UnreservedByUsername);

public static class UnreserveSkuHandler
{
    public static async Task<SkuUnreserved> Handle(UnreserveSku command, CatalogDbContext db)
    {
        var existingSku = await db.SkuReservations.FirstOrDefaultAsync(x => x.Unit == command.Unit);

        if (existingSku is null)
            throw new NullReferenceException($"SKU '{existingSku!.Unit}' not found");

        if (existingSku.IsReserved is false)
            throw new Exception($"SKU '{existingSku.Unit}' is not reserved");

        existingSku.IsReserved = false;

        return new SkuUnreserved(existingSku.Unit, command.Username);
    }
}
