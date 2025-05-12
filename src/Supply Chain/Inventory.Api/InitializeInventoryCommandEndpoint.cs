using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api;

public record InitializeInventoryCommand(string Sku);

/// <summary>
/// This is just one way to quickly start an event stream in
/// the Critter Stack. If HTTP endpoints are your primary usage,
/// this is one way to greatly reduce the boilerplate you normally
/// would have to write.
/// </summary>
public static class InitializeInventoryCommandEndpoint
{
    [WolverinePost("/api/inventory")]
    public static (CreationResponse<Guid>, IStartStream) Post(InitializeInventoryCommand command)
    {
        var initialized = new InventoryInitialized(command.Sku); // event
        var start = MartenOps.StartStream<Inventory>(initialized); // event stream starts

        var response = new CreationResponse<Guid>("/api/inventory/" + start.StreamId, start.StreamId);

        return (response, start);
    }
}
