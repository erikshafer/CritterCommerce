using Inventory.Api.Locations;
using Marten;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record ScheduleShipment(string Origin, string Destination);

public static class ScheduleShipmentHandler
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
}
