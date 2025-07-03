using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record CancelShipment(Guid FreightShipmentId, string Reason, DateTime CancelledAt);

public static class CancelShipmentHandler
{
    [AggregateHandler]
    [WolverinePost("/api/freight-shipments/cancel"), Tags("InboundShipments")]
    public static (Events, OutgoingMessages) Handle(CancelShipment command, FreightShipment shipment)
    {
        if (shipment.Status == FreightShipmentStatus.Cancelled)
            throw new InvalidOperationException($"Shipment is already in '{shipment.Status}' status");

        var events = new Events { new FreightShipmentCancelled(command.Reason, command.CancelledAt) };
        var messages = new OutgoingMessages { new InboundShipmentNotification(shipment.Id, "Cancelled") };

        return (events, messages);
    }
}
