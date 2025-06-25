using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inventories;

public record InitializeInventory(string Sku);

/// <summary>
/// This is just one way to quickly start an event stream in
/// the Critter Stack. If HTTP endpoints are your primary usage,
/// this is one way to greatly reduce the boilerplate you normally
/// would have to write.
/// </summary>
public static class InitializeInventoryEndpoint
{
    [WolverinePost("/api/inventory", Name = "InitializeInventory")]
    public static (CreationResponse<Guid>, IStartStream) Post(InitializeInventory command)
    {
        var streamId = Guid.CreateVersion7(); // create stream id
        var initialized = new InventoryInitialized(streamId, command.Sku); // event
        var start = MartenOps.StartStream<InventoryItem>(initialized); // event stream starts

        var response = new CreationResponse<Guid>("/api/inventory/" + start.StreamId, start.StreamId);

        return (response, start);
    }
}
