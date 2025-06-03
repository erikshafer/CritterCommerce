using Wolverine;
using Wolverine.Marten;

namespace Inventory;

public record InventoryInitialized(Guid Id, string Sku); // evt

public record InventoryMarkedReady; // evt

public record InventoryIncremented(int Quantity); // evt

public record InventoryDecremented(int Quantity); // evt

public class Inventory
{
    public Inventory(InventoryInitialized initialized)
    {
        Sku = new(initialized.Sku);
        Quantity = 0;
        Version = 1;
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
    public static (Events, OutgoingMessages) Handle(PerformInventoryReview review, Inventory inventory)
    {
        var messages = new OutgoingMessages();
        var events = new Events();

        // some <business logic> involving readiness
        // may check another system, require human intervention, etc.
        // and if all looks good...

        events += new InventoryMarkedReady();
        messages.Add(new InventoryReadyForInitialQuantity(inventory.Id));

        // This results in both* new events being captured
        // and the SomeInventoryMessage message going out
        return (events, messages);
    }
}
