using Wolverine.Marten;

namespace Inventory.Api.Receiving;

public record PutawayInboundShipment(Guid ShipmentId, Guid LocationId, string PutawayBy);

public static class PutawayInboundShipmentHandler
{
    [AggregateHandler]
    public static Events Handle(PutawayInboundShipment command, InboundShipment shipment)
    {
        var events = new Events();

        shipment.PutawayShipment(command.LocationId, command.PutawayBy);
        events += new InboundShipmentPutaway(
            command.LocationId,
            command.PutawayBy,
            DateTime.UtcNow
        );

        return events;
    }
}
