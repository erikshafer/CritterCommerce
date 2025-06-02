using Wolverine.Http;
using Wolverine.Persistence;

namespace Catalog.Api.SkuReservations;

public class QuerySkuReservationEndpoints
{
    [WolverineGet("/api/sku-reservation/{id}", Name = "GetSkuReservation")]
    public static SkuReservation Get([Entity] SkuReservation sku) => sku;
}
