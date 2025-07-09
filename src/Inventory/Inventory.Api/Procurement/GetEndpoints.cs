using Marten;
using Marten.Pagination;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;

namespace Inventory.Api.Procurement;

public static class Endpoints
{
    // Here we're not streaming (writing) out raw JSON, so we
    // do not need to explicitly have the [Produces(typeof(ReceivedProcurementOrder))]
    // Wolverine can tell what this is through the [Document] annotation
    // This is convenient if you're not worried about the most performance possible.
    [Tags(Tags.ReceivedProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/{id:int}")]
    public static ReceivedProcurementOrder Get([Document] ReceivedProcurementOrder order) => order;

    // We can't use [Document] here, but through the IQuerySession (a specific derived type
    // of IDocumentSession), we can leverage Marten's LINQ support to construct a query.
    // Nothing fancy, just your friendly ToListAsync() for this async query.
    [Tags(Tags.ReceivedProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received")]
    public static Task<IReadOnlyList<ReceivedProcurementOrder>> GetAll(IQuerySession session) =>
        session.Query<ReceivedProcurementOrder>().ToListAsync();

    // Let's leverage AspDotNet's [FromQuery] attribute to perform a query with Marten
    [Tags(Tags.ReceivedProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/origin/{origin}")]
    public static Task<IReadOnlyList<ReceivedProcurementOrder>> GetAllByOrigin(
        IDocumentSession session,
        [FromQuery] string origin) =>
        session.Query<ReceivedProcurementOrder>()
            .Where(x => x.Origin == origin)
            .ToListAsync();

    // Two parameters
    [Tags(Tags.ReceivedProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/origin/{origin}/destination/{destination}")]
    public static Task<IReadOnlyList<ReceivedProcurementOrder>> GetAllByOrigin(
        IDocumentSession session,
        [FromQuery] string origin,
        [FromQuery] string destination) =>
        session.Query<ReceivedProcurementOrder>()
            .Where(x => x.Origin == origin && x.Destination == destination)
            .ToListAsync();

    [Tags(Tags.ReceivedProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/paged/{page:int}/{pageSize:int}")]
    public static Task<IPagedList<ReceivedProcurementOrder>> GetAllPaged(
        IQuerySession session,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken ct = default)
    {
        IQueryable<ReceivedProcurementOrder> queryable = session
            .Query<ReceivedProcurementOrder>()
            // To make the paging deterministic (DX, HCI, etc)
            .OrderBy(x => x.Id);

        // Marten specific LINQ helper
        return queryable.ToPagedListAsync(page, pageSize, ct);
    }

    public record ReceivedProcurementOrderPageQuery(int PageSize, int PageNumber);

    [Tags(Tags.ReceivedProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/paged/query")]
    public static Task<IPagedList<ReceivedProcurementOrder>> GetAllPagedQuery(
        [FromQuery] ReceivedProcurementOrderPageQuery query,
        IQuerySession session,
        CancellationToken ct = default)
    {
        IQueryable<ReceivedProcurementOrder> queryable = session
            .Query<ReceivedProcurementOrder>()
            // To make the paging deterministic (DX, HCI, etc)
            .OrderBy(x => x.Id);

        // Marten specific LINQ helper
        return queryable.ToPagedListAsync(query.PageNumber, query.PageSize, ct);
    }
}
