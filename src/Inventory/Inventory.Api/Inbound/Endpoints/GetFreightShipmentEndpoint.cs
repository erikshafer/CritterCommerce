using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Inventory.Api.Inbound.Endpoints;

public static class GetFreightShipmentEndpoint
{
    [Produces(typeof(FreightShipment))]
    [WolverineGet("/api/freight-shipments/{id}"), Tags(Tags.InboundShipments)]
    public static async Task Get(Guid id, IDocumentSession session, HttpContext context)
    {
        await session.Json.WriteById<FreightShipment>(id, context);
    }
}
