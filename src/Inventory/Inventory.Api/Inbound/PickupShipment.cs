using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record PickupShipment(Guid FreightShipmentId, DateTime PickedupAt);

public static class PickupShipmentHandler
{
    [AggregateHandler]
    [WolverinePost("/api/freight-shipments/pickup"), Tags("InboundShipments")]
    public static (AcceptResponse, Events) Handle(PickupShipment command, FreightShipment shipment)
    {
        // if (shipment.Status != FreightShipmentStatus.Scheduled)
        //     throw new InvalidOperationException("Cannot pick up unscheduled shipment");

        // TODO
        var messages = new OutgoingMessages { new InboundShipmentNotification(shipment.Id, "PickedUp") };
        var events = new Events { new FreightShipmentPickedUp(command.PickedupAt) };

        return (
            new AcceptResponse($"/api/freight-shipments/{shipment.Id}"),
            [new InboundShipmentNotification(shipment.Id, "PickedUp")]
        );
    }
}
