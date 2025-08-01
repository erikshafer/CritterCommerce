using Marten;
using Marten.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace Inventory.Vendors;

public static class GetEndpoints
{
    [Produces(typeof(Vendor))]
    [Tags(Tags.Vendors)]
    [WolverineGet("/api/vendors/{id:int}")]
    public static async Task Get(int id, IQuerySession session, HttpContext context) =>
        await session.Json.WriteById<Vendor>(id, context);

    [Produces(typeof(Vendor[]))]
    [Tags(Tags.Vendors)]
    [WolverineGet("/api/vendors")]
    public static async Task GetAll([FromServices] IQuerySession session, HttpContext context) =>
        await session.Query<Vendor>().WriteArray(context);
}
