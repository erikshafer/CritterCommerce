using Marten.Schema;

namespace Inventory.Api.Inbound;

public record FreightShipmentScheduled(Guid ShipmentId, string Origin, string Destination, DateTime ScheduledAt);
public record FreightShipmentPickedUp(DateTime PickedUpAt);
public record FreightShipmentDelivered(DateTime DeliveredAt);
public record FreightShipmentCancelled(string Reason, DateTime CancelledAt);

public class FreightShipment
{
    [Identity]
    public Guid Id { get; private set; }
    public string Origin { get; private set; } = null!;
    public string Destination { get; private set; } = null!;
    public FreightShipmentStatus Status { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public DateTime? PickedUpAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    public static FreightShipment Create(FreightShipmentScheduled @event)
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

    public static FreightShipment Apply(FreightShipment current, FreightShipmentPickedUp @event)
    {
        current.Status = FreightShipmentStatus.InTransit;
        current.PickedUpAt = @event.PickedUpAt;
        return current;
    }

    public static FreightShipment Apply(FreightShipment current, FreightShipmentDelivered @event)
    {
        current.Status = FreightShipmentStatus.Delivered;
        current.DeliveredAt = @event.DeliveredAt;
        return current;
    }

    public static FreightShipment Apply(FreightShipment current, FreightShipmentCancelled @event)
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
