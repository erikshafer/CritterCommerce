using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record DeliverShipment(Guid ShipmentId, DateTime DeliveredAt);

public static class DeliverShipmentHandler
{
    [AggregateHandler]
    public static IEnumerable<object> Handle(DeliverShipment cmd, FreightShipment shipment)
    {
        if (shipment.Status != FreightShipmentStatus.Delivered)
            throw new InvalidOperationException($"Shipment is already in '{shipment.Status}' status");

        yield return new FreightShipmentDelivered(cmd.DeliveredAt);
    }
}
