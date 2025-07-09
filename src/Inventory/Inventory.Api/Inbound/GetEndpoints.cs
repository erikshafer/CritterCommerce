using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Inventory.Api.Inbound;

public static class GetEndpoints
{
    [Produces(typeof(FreightShipment))]
    [Tags(Tags.InboundShipments)]
    [WolverineGet("/api/freight-shipments/{id:guid}")]
    public static async Task Get(Guid id, IQuerySession session, HttpContext context) =>
        await session.Json.WriteById<FreightShipment>(id, context);
}
