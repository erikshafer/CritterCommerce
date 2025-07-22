using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.CreateShipments;

public sealed record CreateShipment(Guid Id, string CreatedBy);

public static class CreateShipmentHandler
{
    // Example attribute wiring for Wolverine HTTP API
    [WolverinePost("/api/receiving-shipments/create")]
    [AggregateHandler]
    public static object Handle(CreateShipment command)
    {
        // Business logic and return events or result.
        // To be implemented.
        return new { Success = true };
    }
}
