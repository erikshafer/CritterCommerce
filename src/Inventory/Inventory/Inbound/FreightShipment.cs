using Inventory.Locations;
using Marten;
using Wolverine.Marten;

namespace Inventory.Inbound;

/* events */

public record FreightShipmentScheduled(Guid ShipmentId, string Origin, string Destination, DateTime ScheduledAt);
public record FreightShipmentPickedUp(DateTime PickedUpAt);
public record FreightShipmentDelivered(DateTime DeliveredAt);
public record FreightShipmentCancelled(string Reason, DateTime CancelledAt);

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

    public bool HasBeenPickedUp() => PickedUpAt.HasValue;

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

/* commands */

public record ScheduleShipment(string Origin, string Destination);
public record PickupShipment(Guid ShipmentId, DateTime PickedupAt);
public record DeliverShipment(Guid ShipmentId, DateTime DeliveredAt);
public record CancelShipment(Guid ShipmentId, string Reason, DateTime CancelledAt);

/* event notifications */

public record NotifyDispatchCenter(Guid ShipmentId, string Pickedup);

/* command handlers */

public static class FreightShipmentHandler
{
    [AggregateHandler]
    public static IEnumerable<object> Handle(ScheduleShipment cmd, FreightShipment shipment, IDocumentSession session)
    {
        if (shipment.Status is FreightShipmentStatus.Delivered
                            or FreightShipmentStatus.Cancelled
                            or FreightShipmentStatus.InTransit)
            throw new InvalidOperationException($"Cannot schedule shipment as FreightShipment status is in '{shipment.Status}' status");

        if (string.IsNullOrWhiteSpace(cmd.Origin))
            throw new InvalidOperationException("Cannot schedule shipment with a null Origin value");

        if (string.IsNullOrWhiteSpace(cmd.Destination))
            throw new InvalidOperationException("Cannot schedule shipment with a null Destination value");

        // Can optimize with batched queries, compiled queries, or batched compiled queries! :)
        var originLocation =  session.Query<Location>().FirstOrDefault(x => x.Name == cmd.Origin);
        var destinationLocation = session.Query<Location>().FirstOrDefault(x => x.Name == cmd.Destination);

        if (originLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{cmd.Origin}' in our records");

        if (destinationLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{cmd.Destination}' in our records");

        var id = Guid.NewGuid();
        var scheduledAt = DateTime.UtcNow;

        yield return new FreightShipmentScheduled(id, shipment.Origin, shipment.Destination, scheduledAt);
    }

    [AggregateHandler]
    public static IEnumerable<object> Handle(PickupShipment cmd, FreightShipment shipment)
    {
        if (shipment.Status != FreightShipmentStatus.Scheduled)
            throw new InvalidOperationException("Cannot pick up unscheduled shipment");

        yield return new FreightShipmentPickedUp(cmd.PickedupAt);
        yield return new NotifyDispatchCenter(shipment.Id, "PickedUp");
    }

    [AggregateHandler]
    public static IEnumerable<object> Handle(DeliverShipment cmd, FreightShipment shipment)
    {
        if (shipment.Status != FreightShipmentStatus.Delivered)
            throw new InvalidOperationException($"Shipment is already in '{shipment.Status}' status");

        yield return new FreightShipmentDelivered(cmd.DeliveredAt);
    }

    [AggregateHandler]
    public static IEnumerable<object> Handle(CancelShipment cmd, FreightShipment shipment)
    {
        if (shipment.Status != FreightShipmentStatus.Cancelled)
            throw new InvalidOperationException($"Shipment is already in '{shipment.Status}' status");

        yield return new FreightShipmentCancelled(cmd.Reason, cmd.CancelledAt);
    }
}
