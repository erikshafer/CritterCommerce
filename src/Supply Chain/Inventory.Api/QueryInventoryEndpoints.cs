using Wolverine.Http;
using Wolverine.Persistence;

namespace Inventory.Api;

public static class QueryInventoryEndpoints
{
    [WolverineGet("/api/inventory/{id}", Name = "GetInventory")]
    public static Inventory Get([Entity] Inventory inventory) => inventory;
}
