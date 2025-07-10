using Marten.Events.Projections;

namespace Inventory.Api.Inbound.Views;

public sealed record DailyShipmentsDeliveredView
{
    public string Id { get; set; } = null!;
    public DateOnly DeliveredDate { get; set; }
    public int DeliveredCount { get; set; }
}

public class DailyShipmentsProjection : MultiStreamProjection<DailyShipmentsDeliveredView, string>
{
    public DailyShipmentsProjection()
    {
        // Group events by the DateOnly key as a string (extracted from DeliveredAt)
        Identity<FreightShipmentDelivered>(e => e.DeliveredAt.ToString("yyyy-MM-dd"));
    }

    public DailyShipmentsDeliveredView Create(FreightShipmentDelivered @event)
    {
        // Create a new view for the date if none exists
        return new DailyShipmentsDeliveredView
        {
            Id = @event.DeliveredAt.ToString("yyyy-MM-dd"),
            DeliveredDate = DateOnly.FromDateTime(@event.DeliveredAt),
            DeliveredCount = 1
        };
    }

    public void Apply(FreightShipmentDelivered @event, DailyShipmentsDeliveredView view)
    {
        // Increment the count for this date
        view.DeliveredCount += 1;
    }
}
