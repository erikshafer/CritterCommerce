using Marten;
using Marten.Pagination;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Http.Marten;

namespace Inventory.Procurement;

public static class Endpoints
{
    // Here we're not streaming (writing) out raw JSON, so we
    // do not need to explicitly have the [Produces(typeof(ReceivedProcurementOrder))]
    // Wolverine can tell what this is through the [Document] annotation
    // This is convenient if you're not worried about the most performance possible.
    [Tags(Tags.ProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/{id:int}")]
    public static ProcurementOrder Get([Document] ProcurementOrder order) => order;

    // We can't use [Document] here, but through the IQuerySession (a specific derived type
    // of IDocumentSession), we can leverage Marten's LINQ support to construct a query.
    // Nothing fancy, just your friendly ToListAsync() for this async query.
    [Tags(Tags.ProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received")]
    public static Task<IReadOnlyList<ProcurementOrder>> GetAll(IQuerySession session) =>
        session.Query<ProcurementOrder>().ToListAsync();

    // Let's leverage AspDotNet's [FromQuery] attribute to perform a query with Marten
    [Tags(Tags.ProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/origin/{origin}")]
    public static Task<IReadOnlyList<ProcurementOrder>> GetAllByOrigin(
        IDocumentSession session,
        [FromQuery] string origin) =>
        session.Query<ProcurementOrder>()
            .Where(x => x.Origin == origin)
            .ToListAsync();

    // Two parameters
    [Tags(Tags.ProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/origin/{origin}/destination/{destination}")]
    public static Task<IReadOnlyList<ProcurementOrder>> GetAllByOrigin(
        IDocumentSession session,
        [FromQuery] string origin,
        [FromQuery] string destination) =>
        session.Query<ProcurementOrder>()
            .Where(x => x.Origin == origin && x.Destination == destination)
            .ToListAsync();

    [Tags(Tags.ProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/paged/{page:int}/{pageSize:int}")]
    public static Task<IPagedList<ProcurementOrder>> GetAllPaged(
        IQuerySession session,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken ct = default)
    {
        IQueryable<ProcurementOrder> queryable = session
            .Query<ProcurementOrder>()
            // To make the paging deterministic (DX, HCI, etc)
            .OrderBy(x => x.Id);

        // Marten specific LINQ helper
        return queryable.ToPagedListAsync(page, pageSize, ct);
    }

    public record ReceivedProcurementOrderPageQuery(int PageSize, int PageNumber);

    [Tags(Tags.ProcurementOrders)]
    [WolverineGet("/api/procurement/orders/received/paged/query")]
    public static Task<IPagedList<ProcurementOrder>> GetAllPagedQuery(
        [FromQuery] ReceivedProcurementOrderPageQuery query,
        IQuerySession session,
        CancellationToken ct = default)
    {
        IQueryable<ProcurementOrder> queryable = session
            .Query<ProcurementOrder>()
            // To make the paging deterministic (DX, HCI, etc)
            .OrderBy(x => x.Id);

        // Marten specific LINQ helper
        return queryable.ToPagedListAsync(query.PageNumber, query.PageSize, ct);
    }
}
