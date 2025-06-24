using Marten.Events.Projections;

namespace Inventory.Inbound;

public class DailyShipmentsDelivered
{
    public string Id { get; set; } = null!;
    public DateOnly DeliveredDate { get; set; }
    public int DeliveredCount { get; set; }
}

public class DailyShipmentsProjection : MultiStreamProjection<DailyShipmentsDelivered, string>
{
    public DailyShipmentsProjection()
    {
        // Group events by the DateOnly key as string (extracted from DeliveredAt)
        Identity<FreightShipmentDelivered>(e => e.DeliveredAt.ToString("yyyy-MM-dd"));
    }

    public DailyShipmentsDelivered Create(FreightShipmentDelivered @event)
    {
        // Create a new view for the date if none exists
        return new DailyShipmentsDelivered
        {
            Id = @event.DeliveredAt.ToString("yyyy-MM-dd"),
            DeliveredDate = DateOnly.FromDateTime(@event.DeliveredAt),
            DeliveredCount = 1
        };
    }

    public void Apply(FreightShipmentDelivered @event, DailyShipmentsDelivered view)
    {
        // Increment the count for this date
        view.DeliveredCount += 1;
    }
}
