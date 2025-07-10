using JasperFx.Events;
using Marten.Events.Aggregation;

namespace Inventory.Api.Inbound.Views;

public class FreightShipmentView
{
    public Guid Id { get; set; }
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public string? Status { get; set; }
}

public class FreightShipmentProjection : SingleStreamProjection<FreightShipmentView, Guid>
{
    public FreightShipmentView Create(IEvent<FreightShipmentScheduled> @event) => new FreightShipmentView
    {
        Id = @event.StreamId,
        Origin = @event.Data.Origin,
        Destination = @event.Data.Destination,
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
