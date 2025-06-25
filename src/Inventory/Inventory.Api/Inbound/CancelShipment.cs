using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record CancelShipment(Guid ShipmentId, string Reason, DateTime CancelledAt);

public static class CancelShipmentHandler
{
    [AggregateHandler]
    public static IEnumerable<object> Handle(CancelShipment cmd, FreightShipment shipment)
    {
        if (shipment.Status != FreightShipmentStatus.Cancelled)
            throw new InvalidOperationException($"Shipment is already in '{shipment.Status}' status");

        yield return new FreightShipmentCancelled(cmd.Reason, cmd.CancelledAt);
    }
}
