using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record PickupShipment(Guid ShipmentId, DateTime PickedupAt);

public static class PickupShipmentHandler
{
    [AggregateHandler]
    public static IEnumerable<object> Handle(PickupShipment cmd, FreightShipment shipment)
    {
        if (shipment.Status != FreightShipmentStatus.Scheduled)
            throw new InvalidOperationException("Cannot pick up unscheduled shipment");

        yield return new FreightShipmentPickedUp(cmd.PickedupAt);
        yield return new NotifyDispatchCenter(shipment.Id, "PickedUp");
    }
}
