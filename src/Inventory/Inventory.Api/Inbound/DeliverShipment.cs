using Wolverine;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record DeliverShipment(DateTime DeliveredAt);

public static class DeliverShipmentHandler
{
    [WolverinePost("/api/freight-shipments/{id}/deliver"), Tags("InboundShipments")]
    public static (Events, OutgoingMessages) Handle(DeliverShipment command, [Aggregate] FreightShipment shipment)
    {
        if (shipment.Status == FreightShipmentStatus.Delivered)
            throw new InvalidOperationException($"Shipment is already in '{shipment.Status}' status");

        var events = new Events { new FreightShipmentDelivered(command.DeliveredAt) };
        var messages = new OutgoingMessages { new InboundShipmentNotification(shipment.Id, "Delivered") };

        return (events, messages);
    }
}
