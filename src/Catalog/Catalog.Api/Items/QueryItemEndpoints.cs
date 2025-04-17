using Wolverine.Http;
using Wolverine.Persistence;

namespace Catalog.Api.Items;

public static class QueryItemEndpoints
{
    [WolverineGet("/api/item/{id}")]
    public static Item Get([Entity] Item item) => item;
}
