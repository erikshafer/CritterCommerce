using Marten.Events.Aggregation;

namespace Inventory.Api.Inbound.Projections;

public class ShipmentView
{
    public Guid Id { get; set; }
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public string? Status { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

public class ShipmentViewProjection : SingleStreamProjection<ShipmentView, Guid>
{
    public ShipmentView Create(FreightShipmentScheduled @event) => new ShipmentView
    {
        Id = @event.ShipmentId,
        Origin = @event.Origin,
        Destination = @event.Destination,
        Status = "Scheduled"
    };

    public void Apply(ShipmentView view, FreightShipmentPickedUp @event)
    {
        view.Status = "InTransit";
        view.PickedUpAt = @event.PickedUpAt;
    }

    public void Apply(ShipmentView view, FreightShipmentDelivered @event)
    {
        view.Status = "Delivered";
        view.DeliveredAt = @event.DeliveredAt;
    }
}
