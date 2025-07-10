using JasperFx.Events;

namespace Inventory.Api.Inbound;

public sealed record FreightShipmentScheduled(string Origin, string Destination, DateTime ScheduledAt);
public sealed record FreightShipmentPickedUp(DateTime PickedUpAt);
public sealed record FreightShipmentDelivered(DateTime DeliveredAt);
public sealed record FreightShipmentCancelled(string Reason, DateTime CancelledAt);

public sealed record FreightShipment(
    Guid Id,
    string Origin,
    string Destination,
    FreightShipmentStatus Status,
    DateTime ScheduledAt,
    DateTime? PickedUpAt,
    DateTime? DeliveredAt,
    DateTime? CancelledAt,
    string CancellationReason)
{
    public static FreightShipment Create(IEvent<FreightShipmentScheduled> @event) =>
        new (
            @event.StreamId,
            @event.Data.Origin,
            @event.Data.Destination,
            FreightShipmentStatus.Scheduled,
            @event.Data.ScheduledAt,
            null,
            null,
            null,
            null!
        );

    public static FreightShipment Apply(FreightShipment current, FreightShipmentPickedUp @event) =>
        current with
        {
            Status = FreightShipmentStatus.InTransit,
            PickedUpAt = @event.PickedUpAt
        };

    public static FreightShipment Apply(FreightShipment current, FreightShipmentDelivered @event) =>
        current with
        {
            Status = FreightShipmentStatus.Delivered,
            DeliveredAt = @event.DeliveredAt
        };

    public static FreightShipment Apply(FreightShipment current, FreightShipmentCancelled @event) =>
        current with
        {
            Status = FreightShipmentStatus.Cancelled,
            CancelledAt = @event.CancelledAt,
            CancellationReason = @event.Reason
        };
}

public enum FreightShipmentStatus
{
    Scheduled = 1,
    InTransit = 2,
    Delivered = 4,
    Cancelled = 8
}
