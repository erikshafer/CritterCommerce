using Inventory.Api.Locations;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inbound.Endpoints;

public static class ScheduleFreightShipmentEndpoint
{
    public static ProblemDetails Validate(FreightShipment shipment)
    {
        return shipment.Status == FreightShipmentStatus.Scheduled
            ? new ProblemDetails { Status = StatusCodes.Status400BadRequest, Detail = "Shipment has already been scheduled" }
            : WolverineContinue.NoProblems;
    }

    [WolverinePost("/api/freight-shipments/schedule")]
    public static (CreationResponse<Guid>, IStartStream) Post(ScheduleShipment command, IDocumentSession session)
    {
        var (origin, destination) = command;

        // Can optimize with batched queries, compiled queries, or batched compiled queries! :)
        var originLocation =  session.Query<Location>().FirstOrDefault(x => x.Name == command.Origin);
        var destinationLocation = session.Query<Location>().FirstOrDefault(x => x.Name == command.Destination);

        if (originLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{command.Origin}' in our records");

        if (destinationLocation is null)
            throw new InvalidOperationException($"Cannot locate Origin of '{command.Destination}' in our records");

        var id = CombGuidIdGeneration.NewGuid();
        var scheduledAt = DateTime.UtcNow;
        var scheduled = new FreightShipmentScheduled(id, origin, destination, scheduledAt);
        var start = MartenOps.StartStream<FreightShipment>(scheduled);

        var response = new CreationResponse<Guid>("/api/freight-shipments/" + start.StreamId, start.StreamId);

        return (response, start);
    }
}
