using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record DeliverShipment(Guid FreightShipmentId, DateTime DeliveredAt);

public static class DeliverShipmentHandler
{
    [AggregateHandler]
    [WolverinePost("/api/freight-shipments/deliver"), Tags("InboundShipments")]
    public static (Events, OutgoingMessages) Handle(DeliverShipment cmd, FreightShipment shipment)
    {
        if (shipment.Status != FreightShipmentStatus.Delivered)
            throw new InvalidOperationException($"Shipment is already in '{shipment.Status}' status");

        var events = new Events { new FreightShipmentDelivered(cmd.DeliveredAt) };
        var messages = new OutgoingMessages { new InboundShipmentNotification(shipment.Id, "Delivered") };

        return (events, messages);
    }
}
