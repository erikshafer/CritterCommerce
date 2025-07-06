namespace Inventory.Api.Inbound;

public record FreightShipmentScheduled(Guid Id, string Origin, string Destination, DateTime ScheduledAt);
public record FreightShipmentPickedUp(DateTime PickedUpAt);
public record FreightShipmentDelivered(DateTime DeliveredAt);
public record FreightShipmentCancelled(string Reason, DateTime CancelledAt);

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
    public FreightShipment() : this(Guid.Empty, string.Empty, string.Empty, FreightShipmentStatus.Scheduled, DateTime.MinValue, null, null, null, null!)
    {
    }

    public static FreightShipment Create(FreightShipmentScheduled @event) =>
        new (
            @event.Id,
            @event.Origin,
            @event.Destination,
            FreightShipmentStatus.Scheduled,
            @event.ScheduledAt,
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
            PickedUpAt = @event.DeliveredAt
        };

    public static FreightShipment Apply(FreightShipment current, FreightShipmentCancelled @event) =>
        current with
        {
            Status = FreightShipmentStatus.Cancelled,
            PickedUpAt = @event.CancelledAt,
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
