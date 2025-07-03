using Wolverine.Http;
using Wolverine.Http.Marten;

namespace Inventory.Api.Procurement.Endpoints;

public static class ReceivedProcurementOrderEndpoints
{
    [WolverineGet("/api/procurement/orders/received/{id}"), Tags("ReceivedProcurementOrders")]
    public static ReceivedProcurementOrder Get([Document] ReceivedProcurementOrder order) => order;
}
