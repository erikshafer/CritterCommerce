using Marten.Events.Aggregation;

namespace Inventory.Api.Inbound.Projections;

public class FreightShipmentView
{
    public Guid Id { get; set; }
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public string? Status { get; set; }
}

public class FreightShipmentProjection : SingleStreamProjection<FreightShipmentView, Guid>
{
    public FreightShipmentView Create(FreightShipmentScheduled @event) => new FreightShipmentView
    {
        Id = @event.Id,
        Origin = @event.Origin,
        Destination = @event.Destination,
        Status = "Scheduled"
    };

    public void Apply(FreightShipmentView view, FreightShipmentPickedUp @event)
    {
        view.Status = "InTransit";
    }

    public void Apply(FreightShipmentView view, FreightShipmentDelivered @event)
    {
        view.Status = "Delivered";
    }
}
