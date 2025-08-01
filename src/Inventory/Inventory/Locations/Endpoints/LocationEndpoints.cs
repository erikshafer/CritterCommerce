using Marten;
using Wolverine.Http;
using Wolverine.Http.Marten;

namespace Inventory.Locations.Endpoints;

public static class LocationEndpoints
{
    [WolverineGet("/api/locations"), Tags("Locations")]
    public static Task<IReadOnlyList<Location>> GetAll(IQuerySession session) => session.Query<Location>().ToListAsync();

    [WolverineGet("/api/locations/{id}"), Tags("Locations")]
    public static Location Get([Document] Location location) => location;

    [WolverineGet("/api/locations/soft-delete/{id}"), Tags("Locations")]
    public static Location GetSoftDeleted([Document(Required = true, MaybeSoftDeleted = false)] Location location) =>
        location;

    [WolverineGet("/api/locations/name/{name}"), Tags("Locations")]
    public static Task<Location> GetByName(string name, IQuerySession session) =>
        session.Query<Location>().FirstAsync(x => x.Name == name);

    [WolverineGet("/api/locations/code/{code}"), Tags("Locations")]
    public static Task<Location?> GetByCode(string code, IQuerySession session) =>
        session.Query<Location>().FirstOrDefaultAsync(x => x.Code == code);
}
