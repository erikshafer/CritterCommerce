using Wolverine.Marten;

namespace Inventory.Inbound;

/* events */

public record ShipmentScheduled(Guid ShipmentId, string Origin, string Destination, DateTime ScheduledAt);
public record ShipmentPickedUp(DateTime PickedUpAt);
public record ShipmentDelivered(DateTime DeliveredAt);
public record ShipmentCancelled(string Reason, DateTime CancelledAt);

public class FreightShipment
{
    public Guid Id { get; private set; }
    public string Origin { get; private set; } = null!;
    public string Destination { get; private set; } = null!;
    public FreightShipmentStatus Status { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public DateTime? PickedUpAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    public static FreightShipment Create(ShipmentScheduled @event)
    {
        return new FreightShipment
        {
            Id = @event.ShipmentId,
            Origin = @event.Origin,
            Destination = @event.Destination,
            Status = FreightShipmentStatus.Scheduled,
            ScheduledAt = @event.ScheduledAt
        };
    }

    public static FreightShipment Apply(FreightShipment current, ShipmentPickedUp @event)
    {
        current.Status = FreightShipmentStatus.InTransit;
        current.PickedUpAt = @event.PickedUpAt;
        return current;
    }

    public static FreightShipment Apply(FreightShipment current, ShipmentDelivered @event)
    {
        current.Status = FreightShipmentStatus.Delivered;
        current.DeliveredAt = @event.DeliveredAt;
        return current;
    }

    public static FreightShipment Apply(FreightShipment current, ShipmentCancelled @event)
    {
        current.Status = FreightShipmentStatus.Cancelled;
        current.CancelledAt = @event.CancelledAt;
        current.CancellationReason = @event.Reason;
        return current;
    }
}

public enum FreightShipmentStatus
{
    Scheduled = 1,
    InTransit = 2,
    Delivered = 4,
    Cancelled = 8
}

/* commands */

public record NotifyDispatchCenter(Guid ShipmentId, string Pickedup);
public record PickupShipment(DateTime Timestamp);

/* command handlers */

public static class FreightShipmentHandler
{
    [AggregateHandler]
    public static IEnumerable<object> Handle(PickupShipment cmd, FreightShipment shipment)
    {
        if (shipment.Status != FreightShipmentStatus.Scheduled)
            throw new InvalidOperationException("Cannot pick up unscheduled shipment");

        yield return new ShipmentPickedUp(cmd.Timestamp);
        yield return new NotifyDispatchCenter(shipment.Id, "PickedUp");
    }
}
