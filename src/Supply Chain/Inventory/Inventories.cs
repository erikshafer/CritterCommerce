using Marten;
using Wolverine;
using Wolverine.Marten;

namespace Inventory;

public record InventoryInitialized(string Sku);

public record InventoryMarkedUsable(int StartingQuantity);

public record InventoryIncremented(int Quantity);

public record InventoryDecremented(int Quantity);

public record PhysicalInventoryCountCorrection(int Quantity);

public enum InventoryStatus
{
    Unset = 0,
    Initialized = 1,
    Usable = 2
}

public class Inventory
{
    public Inventory(InventoryInitialized initialized)
    {
        Sku = new(initialized.Sku);
        Quantity = Quantity.IsZero;
        Status = InventoryStatus.Initialized;
    }

    public Guid Id { get; set; }
    public int Version { get; set; }
    public Sku Sku { get; set; }
    public Quantity Quantity { get; set; }
    public InventoryStatus Status { get; set; }

    public bool IsUsable() => Status == InventoryStatus.Usable;
    public bool HasStock() => Quantity.Value > 0;

    public void Apply(InventoryMarkedUsable usable)
    {
        Quantity = new (usable.StartingQuantity);
        Status = InventoryStatus.Usable;
    }

    public void Apply(InventoryIncremented incremented)
    {
        Quantity = Quantity.Add(incremented.Quantity);
    }

    public void Apply(InventoryDecremented decremented)
    {
        Quantity = Quantity.Subtract(decremented.Quantity);
    }

    public void Apply(PhysicalInventoryCountCorrection correction)
    {
        Quantity = new(correction.Quantity);
    }
}

public record MarkInventoryAsUsable(Guid InventoryId, int StartingQuantity);

public static class MarkInventoryAsUsableHandler1
{
    public static async Task Handle1(MarkInventoryAsUsable command, IDocumentSession session)
    {
        var stream = await session
            .Events
            .FetchForExclusiveWriting<Inventory>(command.InventoryId);
        var inventory = stream.Aggregate;

        if (inventory?.IsUsable() is false)
            stream.AppendOne(new InventoryMarkedUsable(command.StartingQuantity));

        await session.SaveChangesAsync();
    }
}

public static class MarkInventoryAsUsableHandler2
{
    [AggregateHandler]
    public static IEnumerable<object> Handle(MarkInventoryAsUsable command, Inventory inventory)
    {
        if (inventory.IsUsable())
            yield break; // just a crude (i.e., bad) example

        yield return new InventoryMarkedUsable(command.StartingQuantity);
    }
}

public record Data;

public interface ISomeService
{
    Task<Data> FindDataAsync();
}

public record SomeInventoryMessage(Guid InventoryId);

public static class MarkInventoryAsUsableHandler3
{
    [AggregateHandler]
    public static async Task<(Events, OutgoingMessages)> Handle(
        MarkInventoryAsUsable command,
        Inventory inventory,
        ISomeService service)
    {
        var data = await service.FindDataAsync();

        var messages = new OutgoingMessages();
        var events = new Events();

        if (inventory.IsUsable())
        {
            events += new InventoryMarkedUsable(command.StartingQuantity);
            messages.Add(new SomeInventoryMessage(inventory.Id));
        }

        // This results in both* new events being captured
        // and the SomeInventoryMessage message going out
        return (events, messages);
    }
}
