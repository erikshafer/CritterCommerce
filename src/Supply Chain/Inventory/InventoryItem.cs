using JasperFx.Events;
using Wolverine;
using Wolverine.Marten;

namespace Inventory;

public record InventoryInitialized(Guid Id, string Sku);

public record InventoryMarkedReady;

public record InventoryIncremented(int Quantity);

public record InventoryDecremented(int Quantity);

/// <summary>
/// Just a plain ole "aggregate". It has nothing to inherit or interfaced with from Marten.
/// This could be a really rich domain model with several, well guarded and potentially
/// complex operations that may not fit well in a simple + specific "read model" projection.
/// But again, you're not leveraging all the types of projection mechanisms that Marten offers
/// by having this be a bit more "free form" and primarily leveraged for live aggregations
/// when needed. However, that may suit your needs just fine!
///
/// If you try to .Add<>)() this to your projections configuration, Marten will yell at you:
/// The type 'Inventory.Inventory' must be convertible to 'JasperFx.Events.Projections.ProjectionBase'
/// </summary>
public class InventoryItem
{
    public InventoryItem Create(IEvent<InventoryInitialized> initialized)
    {
        return new InventoryItem { Id = initialized.StreamId, Sku = initialized.Data.Sku };
    }

    public Guid Id { get; set; }
    public int Version { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }

    public void Apply(InventoryIncremented incremented)
    {
        Quantity += incremented.Quantity;
    }

    public void Apply(InventoryDecremented decremented)
    {
        Quantity -= decremented.Quantity;
    }
}

public record InventoryReadyForInitialQuantity(Guid InventoryId);

public sealed record PerformInventoryReview(Guid InventoryId, int QuantityChange);

public static class PerformInventoryReviewHandler
{
    [AggregateHandler]
    public static (Events, OutgoingMessages) Handle(PerformInventoryReview review, InventoryItem inventoryItem)
    {
        var messages = new OutgoingMessages();
        var events = new Events();

        // some <business logic> involving readiness
        // may check another system, require human intervention, etc.
        // and if all looks good...

        events += new InventoryMarkedReady();
        messages.Add(new InventoryReadyForInitialQuantity(inventoryItem.Id));

        // This results in both* new events being captured
        // and the SomeInventoryMessage message going out
        return (events, messages);
    }
}
