using Wolverine;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record PickupShipment(Guid FreightShipmentId, DateTime PickedupAt);

public static class PickupShipmentHandler
{
    [AggregateHandler]
    public static (Events, OutgoingMessages) Handle(PickupShipment command, FreightShipment shipment)
    {
        if (shipment.Status != FreightShipmentStatus.Scheduled)
            throw new InvalidOperationException("Cannot pick up unscheduled shipment");

        var messages = new OutgoingMessages { new InboundShipmentNotification(shipment.Id, "PickedUp") };
        var events = new Events { new FreightShipmentPickedUp(command.PickedupAt) };

        return (events, messages);
    }
}
