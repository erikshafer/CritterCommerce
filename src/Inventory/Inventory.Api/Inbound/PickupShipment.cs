using Wolverine;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record PickupShipment(DateTime PickedupAt);

public static class PickupShipmentHandler
{
    [WolverinePost("/api/freight-shipments/{id}/pickup"), Tags("InboundShipments")]
    public static (AcceptResponse, Events) Handle(PickupShipment command, [Aggregate] FreightShipment shipment)
    {
        if (shipment.Status == FreightShipmentStatus.Scheduled)
            throw new InvalidOperationException("Shipment has been picked up and is in transit");

        // TODO
        var messages = new OutgoingMessages { new InboundShipmentNotification(shipment.Id, "PickedUp") };
        var events = new Events { new FreightShipmentPickedUp(command.PickedupAt) };

        return (
            new AcceptResponse($"/api/freight-shipments/{shipment.Id}"),
            [new InboundShipmentNotification(shipment.Id, "PickedUp")]
        );
    }
}
