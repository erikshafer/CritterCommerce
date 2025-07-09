using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Receiving.PuttingAway;

public record PutawayInboundShipment(Guid ReceivedShipmentId, Guid LocationId, string PutawayBy);

public static class PutawayInboundShipmentHandler
{
    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments/{receivedShipmentId}/putaway")]
    [AggregateHandler]
    public static Events Handle(PutawayInboundShipment command, ReceivedShipment shipment)
    {
        var events = new Events();
        var now = DateTime.UtcNow;

        events += new ReceivedShipmentPutaway(command.LocationId, command.PutawayBy, now);

        return events;
    }
}
