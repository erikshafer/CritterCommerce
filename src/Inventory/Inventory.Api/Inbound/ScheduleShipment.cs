using Inventory.Api.Inbound.Queries;
using JasperFx.Core;
using Marten;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public record ScheduleShipment(string Origin, string Destination);

public static class ScheduleShipmentHandler
{
    [WolverinePost("/api/freight-shipments"), Tags(Tags.InboundShipments)]
    public static async Task<(CreationResponse<Guid>, IStartStream)> Post(ScheduleShipment command, IDocumentSession session)
    {
        var (origin, destination) = command;

        // Can optimize with batched queries, compiled queries, or batched compiled queries! :)
        // var originLocation =  session.Query<Location>().FirstOrDefault(x => x.Name == origin);
        // var destinationLocation = session.Query<Location>().FirstOrDefault(x => x.Name == destination);

        var batch = session.CreateBatchQuery();
        var originTask = batch.Query(new FindLocationByName { Name = origin });
        var destinationTask = batch.Query(new FindLocationByName { Name = destination });
        await batch.Execute();

        var originLocation = await originTask;
        if (originLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{origin}' in our records");

        var destinationLocation = await destinationTask;
        if (destinationLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{destination}' in our records");

        var id = CombGuidIdGeneration.NewGuid();
        var scheduledAt = DateTime.UtcNow;
        var scheduled = new FreightShipmentScheduled(
            id,
            originLocation.Name,
            destinationLocation.Name,
            scheduledAt);

        var start = MartenOps.StartStream<FreightShipment>(scheduled);
        var response = new CreationResponse<Guid>("/api/freight-shipments/" + start.StreamId, start.StreamId);

        return (response, start);
    }
}
