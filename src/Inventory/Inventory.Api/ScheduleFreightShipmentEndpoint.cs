using Inventory.Inbound;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api;

public static class ScheduleFreightShipmentEndpoint
{
    public static ProblemDetails Validate(FreightShipment shipment)
    {
        return shipment.Status == FreightShipmentStatus.Scheduled
            ? new ProblemDetails { Status = StatusCodes.Status400BadRequest, Detail = "Shipment has already been scheduled" }
            : WolverineContinue.NoProblems;
    }

    [WolverinePost("/api/freight-shipments/schedule")]
    public static (CreationResponse<Guid>, IStartStream) Post(ScheduleShipment command)
    {
        var (origin, destination) = command;
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var scheduled = new FreightShipmentScheduled(id, origin, destination, now);
        var start = MartenOps.StartStream<FreightShipment>(scheduled);

        var response = new CreationResponse<Guid>("/api/freight-shipments/" + start.StreamId, start.StreamId);

        return (response, start);
    }
}
