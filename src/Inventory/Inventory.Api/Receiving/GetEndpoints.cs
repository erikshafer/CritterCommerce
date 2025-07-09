using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Inventory.Api.Receiving;

public static class GetEndpoints
{
    [Produces(typeof(ReceivedShipment))]
    [Tags(Tags.ReceivingShipments)]
    [WolverineGet("/api/receiving-shipments/{id:guid}")]
    public static async Task Get(Guid id, IQuerySession session, HttpContext context) =>
        await session.Json.WriteById<ReceivedShipment>(id, context);

    [Produces(typeof(ReceivedShipment[]))]
    [Tags(Tags.ReceivingShipments)]
    [WolverineGet("/api/receiving-shipments")]
    public static async Task GetAll([FromServices] IQuerySession session, HttpContext context) =>
        await session.Query<ReceivedShipment>().WriteArray(context);
}
