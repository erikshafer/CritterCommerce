using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Inventory.Api.Inbound;

public static class GetFreightShipmentEndpoint
{
    // For right now, you have to help out the OpenAPI metadata
    [Produces(typeof(FreightShipment))]
    [WolverineGet("/api/freight-shipments/{id}")]
    public static async Task Get(Guid id, IDocumentSession session, HttpContext context)
    {
        await session.Json.WriteById<FreightShipment>(id, context);
    }
}
