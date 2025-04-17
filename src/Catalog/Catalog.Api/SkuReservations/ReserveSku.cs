using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Persistence;

namespace Catalog.Api.SkuReservations;

public sealed record ReserveSku(int Unit);

public sealed record SkuReserved(int Unit, bool Reserved);

public static class ReserveSkuCommandHandler
{
    public static SkuReserved Handle(ReserveSku command, CatalogDbContext db)
    {
        var sku = new SkuReservation { Unit = command.Unit, Reserved = true };

        db.SkuReservations.Add(sku);

        return new SkuReserved(sku.Unit, sku.Reserved);
    }
}

public static class ReserveSkuEndpoints
{
    [Transactional]
    [WolverinePost("/api/sku-reservations")]
    public static (SkuReserved, Insert<SkuReservation>) Post(ReserveSku command)
    {
        var entity = new SkuReservation { Unit = command.Unit, Reserved = true };

        return (new SkuReserved(entity.Unit, entity.Reserved), Storage.Insert(entity));
    }
}
