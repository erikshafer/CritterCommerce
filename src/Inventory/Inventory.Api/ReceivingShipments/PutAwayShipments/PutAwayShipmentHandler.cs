using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.PutAwayShipments;

public sealed record PutAwayShipment(Guid ShipmentId, Guid LocationId, string PutawayBy);

public static class PutAwayShipmentHandler
{
    [WolverinePost("/api/receiving-shipments/{shipmentId}/put-away")]
    [AggregateHandler]
    public static object Handle(PutAwayShipment command)
    {
        // To be implemented
        return new { Success = true };
    }
}
