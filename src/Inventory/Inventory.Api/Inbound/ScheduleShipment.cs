using Inventory.Api.Locations;
using JasperFx.Core;
using Marten;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record ScheduleShipment(string Origin, string Destination);

public static class ScheduleShipmentHandler
{
    [WolverinePost("/api/freight-shipments"), Tags("InboundShipments")]
    public static (CreationResponse<Guid>, IStartStream) Post(ScheduleShipment command, IDocumentSession session)
    {
        var (origin, destination) = command;

        // Can optimize with batched queries, compiled queries, or batched compiled queries! :)
        var originLocation =  session.Query<Location>().FirstOrDefault(x => x.Name == origin);
        var destinationLocation = session.Query<Location>().FirstOrDefault(x => x.Name == destination);

        if (originLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{origin}' in our records");

        if (destinationLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{destination}' in our records");

        var id = CombGuidIdGeneration.NewGuid();
        var scheduledAt = DateTime.UtcNow;
        var scheduled = new FreightShipmentScheduled(id, origin, destination, scheduledAt);

        var start = MartenOps
            .StartStream<FreightShipment>(scheduled);
        var response = new CreationResponse<Guid>("/api/freight-shipments/" + start.StreamId, start.StreamId);

        return (response, start);
    }
}
