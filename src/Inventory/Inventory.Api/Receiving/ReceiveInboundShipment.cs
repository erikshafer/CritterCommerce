using Wolverine.Marten;

namespace Inventory.Api.Receiving;

public record ReceiveInboundShipment(Guid ShipmentId, string ReceivedBy);

public static class ReceivedInboundShipmentHandler
{
    [AggregateHandler]
    public static Events Handle(ReceiveInboundShipment command, InboundShipment shipment)
    {
        var events = new Events();

        shipment.ReceiveShipment(command.ReceivedBy);
        events += new InboundShipmentReceived(command.ReceivedBy, DateTime.UtcNow);

        return events;
    }
}
